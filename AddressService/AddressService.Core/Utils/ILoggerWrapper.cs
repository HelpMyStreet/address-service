using System;

namespace AddressService.Core.Utils
{
    public interface ILoggerWrapper<T>
    {
        void LogError(string message, Exception exception);
        void LogWarning(string message, Exception exception);
        void LogInformation(string message);
    }
}