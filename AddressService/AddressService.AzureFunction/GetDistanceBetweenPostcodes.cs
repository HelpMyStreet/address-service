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
    public class GetDistanceBetweenPostcodes
    {
        private readonly IMediator _mediator;
        private readonly ILoggerWrapper<GetDistanceBetweenPostcodes> _logger;

        public GetDistanceBetweenPostcodes(IMediator mediator, ILoggerWrapper<GetDistanceBetweenPostcodes> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Transaction(Web = true)]
        [FunctionName("GetDistanceBetweenPostcodes")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ResponseWrapper<GetDistanceBetweenPostcodesResponse, AddressServiceErrorCode>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseWrapper<GetDistanceBetweenPostcodesResponse, AddressServiceErrorCode>))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            [RequestBodyType(typeof(GetDistanceBetweenPostcodesRequest), "Get Distance Between Postcodes")] GetDistanceBetweenPostcodesRequest req,
            CancellationToken cancellationToken)
        {

            try
            {
                var input = JsonConvert.SerializeObject(req);
                _logger.LogInformation(input);
                GetDistanceBetweenPostcodesResponse response = await _mediator.Send(req, cancellationToken);
                return new OkObjectResult(ResponseWrapper<GetDistanceBetweenPostcodesResponse, AddressServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                _logger.LogErrorAndNotifyNewRelic("Exception occured in GetDistanceBetweenPostcodes", exc);
                _logger.LogError(exc.ToString(),exc);
                return new ObjectResult(ResponseWrapper<GetDistanceBetweenPostcodesResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}