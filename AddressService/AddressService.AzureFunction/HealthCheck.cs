using AddressService.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using NewRelic.Api.Agent;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;


namespace AddressService.AzureFunction
{
    public class HealthCheck
    {
        private readonly ILoggerWrapper<HealthCheck> _logger;

        public HealthCheck(ILoggerWrapper<HealthCheck> logger)
        {
            _logger = logger;
        }


        [Transaction(Web = true)]
        [FunctionName("HealthCheck")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("AddressService", nameof(HealthCheck));
                _logger.LogInformation("C# HTTP trigger function processed health check request.");

                return new OkObjectResult("I'm alive!");
            }
            catch (Exception ex)
            {
                _logger.LogErrorAndNotifyNewRelic($"Unhandled error in HealthCheck", ex, req);
                return new InternalServerErrorResult();
            }
        }
    }
}
