using System;

namespace AddressService.Core.Utils
{
    public interface ILoggerWrapper<T>
    {
        void LogError(string message, Exception exception);

        void LogError<T>(string message, Exception exception, T request);

        void LogErrorAndNotifyNewRelic(string message, Exception exception);
        
        void LogErrorAndNotifyNewRelic<T>(string message, Exception exception, T request);

        void LogWarning(string message, Exception exception);

        void LogWarning<T>(string message, Exception exception, T request);

        void LogInformation(string message);

        void LogInformation<T>(string message, Exception exception, T request);
    }
}