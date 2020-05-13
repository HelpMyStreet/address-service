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

        public void LogError<T>(string message, Exception exception, T request)
        {
            _logger.LogError(exception, message, request);
        }

        public void LogErrorAndNotifyNewRelic(string message, Exception exception)
        {
            NewRelic.Api.Agent.NewRelic.NoticeError(exception);
            _logger.LogError(message, exception);
        }

        public void LogErrorAndNotifyNewRelic<T>(string message, Exception exception, T request)
        {
            NewRelic.Api.Agent.NewRelic.NoticeError(exception);
            _logger.LogError(exception, message, request);
        }

        public void LogWarning(string message, Exception exception)
        {
            _logger.LogWarning(message, exception);
        }

        public void LogWarning<T>(string message, Exception exception, T request)
        {
            _logger.LogWarning(exception, message, request);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogInformation<T>(string message, Exception exception, T request)
        {
            _logger.LogInformation(exception, message, request);
        }
    }
}
