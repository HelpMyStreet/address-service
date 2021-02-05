using AddressService.Core.Utils;
using AddressService.Core.Validation;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
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
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.AzureFunction
{
    public class GetLocations
    {
        private readonly IMediator _mediator;
        private readonly ILoggerWrapper<GetLocations> _logger;

        public GetLocations(IMediator mediator, ILoggerWrapper<GetLocations> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Transaction(Web = true)]
        [FunctionName("GetLocations")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ResponseWrapper<GetLocationsResponse, AddressServiceErrorCode>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseWrapper<GetLocationsResponse, AddressServiceErrorCode>))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            [RequestBodyType(typeof(GetLocationsRequest), "Get Locations")] GetLocationsRequest req,
            CancellationToken cancellationToken)
        {

            try
            {
                var input = JsonConvert.SerializeObject(req);
                _logger.LogInformation(input);
                GetLocationsResponse response = await _mediator.Send(req, cancellationToken);
                return new OkObjectResult(ResponseWrapper<GetLocationsResponse, AddressServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                _logger.LogErrorAndNotifyNewRelic("Exception occured in GetLocations", exc);
                _logger.LogError(exc.ToString(),exc);
                return new ObjectResult(ResponseWrapper<GetLocationsResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}