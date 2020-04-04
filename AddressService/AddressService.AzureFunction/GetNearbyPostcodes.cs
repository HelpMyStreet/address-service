using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Validation;
using Microsoft.AspNetCore.Http;


namespace AddressService.AzureFunction
{
    public class GetNearbyPostcodes
    {
        private readonly IMediator _mediator;
        private readonly IPostcodeValidator _postcodeValidator;

        public GetNearbyPostcodes(IMediator mediator, IPostcodeValidator postcodeValidator)
        {
            _mediator = mediator;
            _postcodeValidator = postcodeValidator;
        }

        [FunctionName("GetNearbyPostcodes")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            GetNearbyPostcodesRequest req,
            CancellationToken cancellationToken,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                // This validation logic belongs in a custom validation attribute on the Postcode property.  However, validationContext.GetService<IExternalService> always returned null in the validation attribute (despite DI working fine elsewhere).  I didn't want to spend a lot of time finding out why when there is lots to do so I've put the postcode validation logic here for now.
                if (!await _postcodeValidator.IsPostcodeValidAsync(req.Postcode))
                {
                    return new OkObjectResult(ResponseWrapper.CreateUnsuccessfulResponse("Invalid postcode"));
                }

                if (req.IsValid(out var validationResults))
                {
                    GetNearbyPostcodesResponse response = await _mediator.Send(req, cancellationToken);
                    return new OkObjectResult(ResponseWrapper<GetNearbyPostcodesResponse>.CreateSuccessfulResponse(response));
                }
                else
                {
                    return new OkObjectResult(ResponseWrapper.CreateUnsuccessfulResponse(validationResults));
                }
            }
            catch (Exception exc)
            {
                log.LogError(exc, "Unhandled error in GetNearbyPostcodes");
                return new ObjectResult(ResponseWrapper.CreateUnsuccessfulResponse("Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}

