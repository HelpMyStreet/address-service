using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace AddressService.Core.Contracts
{
    public class ResponseWrapper<TContent, TErrorCode> where TErrorCode : struct, IConvertible
    {
        [DataMember(Name = "content")]
        public TContent Content { get; set; }

        public bool HasContent => Content != null;

        public bool HasErrors => Errors.Any();

        public bool IsSuccessful => !HasErrors && HasContent;

        [DataMember(Name = "errors")]
        public IReadOnlyList<Error<TErrorCode>> Errors { get; private set; } = new List<Error<TErrorCode>>();

        public static ResponseWrapper<TContent, TErrorCode> CreateSuccessfulResponse(TContent content)
        {
            ResponseWrapper<TContent, TErrorCode> responseWrapper = new ResponseWrapper<TContent, TErrorCode>()
            {
                Content = content
            };

            return responseWrapper;
        }

        public static ResponseWrapper<TContent, TErrorCode> CreateUnsuccessfulResponse(IEnumerable<Error<TErrorCode>> errors)
        {
            ResponseWrapper<TContent, TErrorCode> responseWrapper = new ResponseWrapper<TContent, TErrorCode>()
            {
                Errors = errors.ToList()
            };

            return responseWrapper;
        }

        public static ResponseWrapper<TContent, TErrorCode> CreateUnsuccessfulResponse(TErrorCode errorCode, string errorMessage)
        {
            ResponseWrapper<TContent, TErrorCode> responseWrapper = new ResponseWrapper<TContent, TErrorCode>()
            {
                Errors = new List<Error<TErrorCode>>() { new Error<TErrorCode>(errorCode, errorMessage) }
            };

            return responseWrapper;
        }

        public static ResponseWrapper<TContent, TErrorCode> CreateUnsuccessfulResponse(TErrorCode validationErrorCode, IEnumerable<ValidationResult> validationResults)
        {
            ResponseWrapper<TContent, TErrorCode> responseWrapper = new ResponseWrapper<TContent, TErrorCode>()
            {
                Errors = validationResults.Select(x => new Error<TErrorCode>(validationErrorCode, x.ErrorMessage)).ToList()
            };

            return responseWrapper;
        }

    }
}