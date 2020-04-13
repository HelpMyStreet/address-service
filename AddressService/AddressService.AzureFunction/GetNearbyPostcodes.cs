using AddressService.Core.Utils;
using AddressService.Core.Validation;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace AddressService.AzureFunction
{
    public class GetNearbyPostcodes
    {
        private readonly IMediator _mediator;
        private readonly IPostcodeValidator _postcodeValidator;
        private readonly ILoggerWrapper<GetNearbyPostcodes> _logger;

        public GetNearbyPostcodes(IMediator mediator, IPostcodeValidator postcodeValidator, ILoggerWrapper<GetNearbyPostcodes> logger)
        {
            _mediator = mediator;
            _postcodeValidator = postcodeValidator;
            _logger = logger;
        }

        [FunctionName("GetNearbyPostcodes")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            GetNearbyPostcodesRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a request.");

                // This validation logic belongs in a custom validation attribute on the Postcode property.  However, validationContext.GetService<IExternalService> always returned null in the validation attribute (despite DI working fine elsewhere).  I didn't want to spend a lot of time finding out why when there is lots to do so I've put the postcode validation logic here for now.
                if (!await _postcodeValidator.IsPostcodeValidAsync(req.Postcode))
                {
                    return new OkObjectResult(ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.InvalidPostcode, "Invalid postcode"));
                }

                if (req.IsValid(out var validationResults))
                {
                    GetNearbyPostcodesResponse response = await _mediator.Send(req, cancellationToken);
                    return new OkObjectResult(ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode>.CreateSuccessfulResponse(response));
                }
                else
                {
                    return new OkObjectResult(ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.ValidationError, validationResults));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Unhandled error in GetNearbyPostcodes", ex);
                return new ObjectResult(ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}

