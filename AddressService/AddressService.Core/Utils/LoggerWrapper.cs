using Microsoft.Extensions.Logging;
using System;

namespace AddressService.Core.Utils
{
    /// <summary>
    /// Wrapper for ILogger to make unit testing easier
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LoggerWrapper<T> : ILoggerWrapper<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerWrapper(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogError(string message, Exception exception)
        {
            _logger.LogError(message, exception);
        }

        public void LogWarning(string message, Exception exception)
        {
            _logger.LogWarning(message, exception);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }
    }
}
