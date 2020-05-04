using AddressService.Core.Utils;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using NewRelic.Api.Agent;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.AzureFunction
{
    public class GetPostcodeCoordinates
    {
        private readonly IMediator _mediator;
        private readonly ILoggerWrapper<GetPostcodeCoordinates> _logger;

        public GetPostcodeCoordinates(IMediator mediator, ILoggerWrapper<GetPostcodeCoordinates> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Transaction(Web = true)]
        [FunctionName("GetPostcodeCoordinates")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ResponseWrapper<GetPostcodeCoordinatesRequest, AddressServiceErrorCode>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseWrapper<GetPostcodeCoordinatesRequest, AddressServiceErrorCode>))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage reqAsHttpRequestMessage,
            CancellationToken cancellationToken)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("AddressService", "GetPostcodeCoordinates");
                _logger.LogInformation("C# HTTP trigger function processed a request.");

                // accept compressed requests (can't do this with middleware)
                GetPostcodeCoordinatesRequest req = await HttpRequestMessageCompressionUtils.DeserialiseAsync<GetPostcodeCoordinatesRequest>(reqAsHttpRequestMessage);

                if (req.IsValid(out var validationResults))
                {
                    GetPostcodeCoordinatesResponse response = await _mediator.Send(req, cancellationToken);
                    return new OkObjectResult(ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>.CreateSuccessfulResponse(response));
                }
                else
                {
                    return new OkObjectResult(ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.ValidationError, validationResults));
                }
            }
            catch (Exception ex)
            {
                LogError.Log(_logger, ex, reqAsHttpRequestMessage);
                return new ObjectResult(ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}