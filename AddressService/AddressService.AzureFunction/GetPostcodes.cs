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
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.AzureFunction
{
    public class GetPostcodes
    {
        private readonly IMediator _mediator;
        private readonly IPostcodeValidator _postcodeValidator;
        private readonly ILoggerWrapper<GetPostcodes> _logger;

        public GetPostcodes(IMediator mediator, IPostcodeValidator postcodeValidator, ILoggerWrapper<GetPostcodes> logger)
        {
            _mediator = mediator;
            _postcodeValidator = postcodeValidator;
            _logger = logger;
        }

        [FunctionName("GetPostcodes")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ResponseWrapper<GetPostcodesResponse, AddressServiceErrorCode>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseWrapper<GetPostcodesResponse, AddressServiceErrorCode>))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] GetPostcodesRequest req,
            CancellationToken cancellationToken)
        {

            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a request.");

                GetPostcodesResponse response = await _mediator.Send(req, cancellationToken);
                return new OkObjectResult(ResponseWrapper<GetPostcodesResponse, AddressServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception ex)
            {
                _logger.LogError("Unhandled error in GetPostcodes", ex);
                return new ObjectResult(ResponseWrapper<GetPostcodesResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.UnhandledError,"Internal Error")) {StatusCode = StatusCodes.Status500InternalServerError};
            }
        }
    }
}