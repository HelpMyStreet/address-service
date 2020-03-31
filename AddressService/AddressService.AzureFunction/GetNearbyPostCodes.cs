﻿using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


namespace AddressService.AzureFunction
{
    public class GetNearbyPostcodes
    {
        private readonly IMediator _mediator;

        public GetNearbyPostcodes(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("GetNearbyPostcodes")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] GetNearbyPostcodesRequest req,
            CancellationToken cancellationToken,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

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
                return new StatusCodeResult(500);
            }
        }
    }
}