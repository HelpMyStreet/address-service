using AddressService.Core.Contracts;
using AddressService.Core.Utils;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.AzureFunction
{
    public class GetNumberOfAddressesPerPostcodeInBoundary
    {
        private readonly IMediator _mediator;
        private readonly ILoggerWrapper<GetNumberOfAddressesPerPostcodeInBoundary> _logger;

        public GetNumberOfAddressesPerPostcodeInBoundary(IMediator mediator, ILoggerWrapper<GetNumberOfAddressesPerPostcodeInBoundary> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [FunctionName("GetNumberOfAddressesInBoundary")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ResponseWrapper<GetNumberOfAddressesPerPostcodeInBoundaryResponse, AddressServiceErrorCode>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseWrapper<GetNumberOfAddressesPerPostcodeInBoundaryResponse, AddressServiceErrorCode>))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] GetNumberOfAddressesPerPostcodeInBoundaryRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a request.");


                if (req.IsValid(out var validationResults))
                {
                    GetNumberOfAddressesPerPostcodeInBoundaryResponse response = await _mediator.Send(req, cancellationToken);
                    return new OkObjectResult(ResponseWrapper<GetNumberOfAddressesPerPostcodeInBoundaryResponse, AddressServiceErrorCode>.CreateSuccessfulResponse(response));
                }
                else
                {
                    return new OkObjectResult(ResponseWrapper<GetNumberOfAddressesPerPostcodeInBoundaryResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.ValidationError, validationResults));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unhandled error in {nameof(GetNumberOfAddressesPerPostcodeInBoundary)}", ex);
                return new ObjectResult(ResponseWrapper<GetNumberOfAddressesPerPostcodeInBoundaryResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}