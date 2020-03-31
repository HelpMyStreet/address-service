using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AddressService.AzureFunction
{
    public class Test
    {
        [FunctionName("Test")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                var config = new ConfigurationBuilder()
        .SetBasePath(Environment.CurrentDirectory)
        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

                string connectionstring = config.GetConnectionString("SqlConnectionString");

                return new OkObjectResult(connectionstring);
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }
    }
}
