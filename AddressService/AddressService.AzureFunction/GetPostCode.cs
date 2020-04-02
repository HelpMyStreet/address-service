﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using System.Diagnostics;
using System.Threading;
using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Utils;

namespace AddressService.AzureFunction
{
    public class GetPostcode
    {
        private readonly IMediator _mediator;

        public GetPostcode(IMediator mediator)
        {
            _mediator = mediator;
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

                if (req.IsValid(out var validationResults))
                {
                    PostcodeResponse response = await _mediator.Send(req, cancellationToken);
                    return new OkObjectResult(ResponseWrapper<PostcodeResponse>.CreateSuccessfulResponse(response));
                }
                else
                {
                    return new OkObjectResult(ResponseWrapper.CreateUnsuccessfulResponse(validationResults));
                }
            }
            catch (Exception exc)
            {
                log.LogError(exc, "Unhandled error in GetPostcode");
                return new StatusCodeResult(500);
            }
        }
    }
}