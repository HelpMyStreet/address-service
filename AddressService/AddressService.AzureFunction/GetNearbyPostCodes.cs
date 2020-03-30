using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Utils;


namespace AddressService.AzureFunction
{
    public class GetNearbyPostCodes
    {
        private readonly IMediator _mediator;

        public GetNearbyPostCodes(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("GetNearbyPostCodes")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] GetNearbyPostCodesRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                if (req.IsValid(out var validationResults))
                {
                    GetNearbyPostCodesResponse response = await _mediator.Send(req);
                    return new OkObjectResult(ResponseWrapper<GetNearbyPostCodesResponse>.CreateSuccessfulResponse(response));
                }
                else
                {
                    return new OkObjectResult(ResponseWrapper.CreateUnsuccessfulResponse(validationResults));
                }
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }
    }
}
