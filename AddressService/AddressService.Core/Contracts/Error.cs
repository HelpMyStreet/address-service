using System;
using System.Runtime.Serialization;

namespace AddressService.Core.Contracts
{
    [DataContract(Name = "error")]
    public class Error<TErrorCode> where TErrorCode : struct, IConvertible
    {
        public Error(TErrorCode errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }


        [DataMember(Name = "errorCode")]
        public TErrorCode ErrorCode { get; set; }

        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
