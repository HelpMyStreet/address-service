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
    public class GetVolunteerCount
    {
        private readonly IMediator _mediator;

        public GetVolunteerCount(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("GetVolunteerCount")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] GetVolunteerCountRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                VolunteerCountResponse response = await _mediator.Send(req);
                return new OkObjectResult(response);
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }
    }
}
