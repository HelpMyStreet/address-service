using AddressService.Core.Utils;
using AddressService.Core.Validation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;

namespace AddressService.AzureFunction
{
    public class GetPostcode
    {
        private readonly IMediator _mediator;
        private readonly IPostcodeValidator _postcodeValidator;

        public GetPostcode(IMediator mediator, IPostcodeValidator postcodeValidator)
        {
            _mediator = mediator;
            _postcodeValidator = postcodeValidator;
        }

        [FunctionName("GetPostcode")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] GetPostcodeRequest req,
            CancellationToken cancellationToken,
            ILogger log)
        {

            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                // This validation logic belongs in a custom validation attribute on the Postcode property.  However, validationContext.GetService<IExternalService> always returned null in the validation attribute (despite DI working fine elsewhere).  I didn't want to spend a lot of time finding out why when there is lots to do so I've put the postcode validation logic here for now.
                if (!await _postcodeValidator.IsPostcodeValidAsync(req.Postcode))
                {
                    return new OkObjectResult(ResponseWrapper<GetPostcodeResponse>.CreateUnsuccessfulResponse("Invalid postcode"));
                }

                if (req.IsValid(out var validationResults))
                {
                    GetPostcodeResponse response = await _mediator.Send(req, cancellationToken);
                    return new OkObjectResult(ResponseWrapper<GetPostcodeResponse>.CreateSuccessfulResponse(response));
                }
                else
                {
                    return new OkObjectResult(ResponseWrapper<GetPostcodeResponse>.CreateUnsuccessfulResponse(validationResults));
                }
            }
            catch (Exception exc)
            {
                log.LogError(exc, "Unhandled error in GetNearbyPostcodes");
                return new ObjectResult(ResponseWrapper<GetPostcodeResponse>.CreateUnsuccessfulResponse("Internal Error")) {StatusCode = StatusCodes.Status500InternalServerError};
            }
        }
    }
}