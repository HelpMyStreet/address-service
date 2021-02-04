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
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.AzureFunction
{
    public class GetLocationsByDistance
    {
        private readonly IMediator _mediator;
        private readonly ILoggerWrapper<GetLocationsByDistance> _logger;

        public GetLocationsByDistance(IMediator mediator, ILoggerWrapper<GetLocationsByDistance> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Transaction(Web = true)]
        [FunctionName("GetLocationsByDistance")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ResponseWrapper<GetLocationsByDistanceResponse, AddressServiceErrorCode>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseWrapper<GetLocationsByDistanceResponse, AddressServiceErrorCode>))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            [RequestBodyType(typeof(GetLocationsByDistanceRequest), "Get Locations By Distance")] GetLocationsByDistanceRequest req,
            CancellationToken cancellationToken)
        {

            try
            {
                GetLocationsByDistanceResponse response = await _mediator.Send(req, cancellationToken);
                return new OkObjectResult(ResponseWrapper<GetLocationsByDistanceResponse, AddressServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                _logger.LogErrorAndNotifyNewRelic("Exception occured in GetLocationsByDistance", exc);
                return new ObjectResult(ResponseWrapper<GetLocationsByDistanceResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}