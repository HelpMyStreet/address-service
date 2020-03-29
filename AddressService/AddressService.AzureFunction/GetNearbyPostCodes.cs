using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using AddressService.Core.Domains.Entities.GetNearbyPostCodes;

namespace AddressService.AzureFunction
{
    public class GetNearbyPostCodes
    {
        private readonly IMediator _mediator;

        public GetNearbyPostCodes(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("FunctionB")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] FunctionBRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                FunctionBResponse response = await _mediator.Send(req);
                return new OkObjectResult(response);
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }
    }
}
