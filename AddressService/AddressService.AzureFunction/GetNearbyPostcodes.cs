using AddressService.Core.Utils;
using AddressService.Core.Validation;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Utils.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using NewRelic.Api.Agent;
using System;
using System.Net;
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

        [Transaction(Web = true)]
        [FunctionName("GetNearbyPostcodes")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode>))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            GetNearbyPostcodesRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("AddressService", "GetNearbyPostcodes");
                _logger.LogInformation("C# HTTP trigger function processed a request.");

                // This validation logic belongs in a custom validation attribute on the Postcode property.  However, validationContext.GetService<IExternalService> always returned null in the validation attribute (despite DI working fine elsewhere).  I didn't want to spend a lot of time finding out why when there is lots to do so I've put the postcode validation logic here for now.
                if (!await _postcodeValidator.IsPostcodeValidAsync(req.Postcode))
                {
                    return new ObjectResult(ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.InvalidPostcode, "Invalid postcode")) { StatusCode = 422 };
                }

                if (req.IsValid(out var validationResults))
                {
                    GetNearbyPostcodesResponse response = await _mediator.Send(req, cancellationToken);
                    return new OkObjectResult(ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode>.CreateSuccessfulResponse(response));
                }
                else
                {
                    return new ObjectResult(ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.ValidationError, validationResults)) { StatusCode = 422 };
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorAndNotifyNewRelic($"Unhandled error in GetNearbyPostcodes", ex, req);
                return new ObjectResult(ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}

