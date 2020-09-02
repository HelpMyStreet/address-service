using AddressService.Core.Utils;
using HelpMyStreet.Utils.Utils;
using Microsoft.Azure.WebJobs;
using System;


namespace AddressService.AzureFunction
{
    public class TimedHealthCheck
    {
        private readonly ILoggerWrapper<TimedHealthCheck> _logger;

        public TimedHealthCheck(ILoggerWrapper<TimedHealthCheck> logger)
        {
            _logger = logger;
        }

        [FunctionName("TimedHealthCheck")]
        public void Run([TimerTrigger("%TimedHealthCheckCronExpression%")] TimerInfo timerInfo)
        {
            _logger.LogInformation($"Health check CRON trigger executed at : {DateTimeOffset.Now}");
        }
    }
}
