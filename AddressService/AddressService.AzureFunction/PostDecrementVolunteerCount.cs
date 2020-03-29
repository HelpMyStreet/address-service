using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;

namespace AddressService.AzureFunction
{
    public class PostDecrementVolunteerCount
    {
        private readonly IMediator _mediator;

        public PostDecrementVolunteerCount(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("PostDecrementVolunteerCount")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] PostDecrementVolunteerCountRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                await _mediator.Send(req);
                return new NoContentResult();
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }
    }
}
