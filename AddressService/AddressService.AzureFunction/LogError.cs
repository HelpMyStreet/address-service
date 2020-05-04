using AddressService.Core.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddressService.AzureFunction
{
    public static class LogError
    {
        public static void Log<T>(ILoggerWrapper<T> log, Exception exc, Object request)
        {
            NewRelic.Api.Agent.NewRelic.NoticeError(exc);
            log.LogError("Unhandled error", exc);
        }
    }
}
