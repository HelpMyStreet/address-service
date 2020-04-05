using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace AddressService.Core.Contracts
{
    public class ResponseWrapper<TErrorCode> where TErrorCode : struct, IConvertible
    {
        public static ResponseWrapper<TErrorCode> CreateSuccessfulResponse()
        {
            var responseWrapper = new ResponseWrapper<TErrorCode>()
            {
                IsSuccessful = true
            };

            return responseWrapper;
        }

        public static ResponseWrapper<TErrorCode> CreateUnsuccessfulResponse(IEnumerable<Error<TErrorCode>> errors)
        {
            var responseWrapper = new ResponseWrapper<TErrorCode>()
            {
                IsSuccessful = false,
                Errors = errors.ToList()
            };

            return responseWrapper;
        }

        public static ResponseWrapper<TErrorCode> CreateUnsuccessfulResponse(TErrorCode errorCode, string errorMessage)
        {
            var responseWrapper = new ResponseWrapper<TErrorCode>()
            {
                IsSuccessful = false,
                Errors = new List<Error<TErrorCode>>() { new Error<TErrorCode>(errorCode, errorMessage) }
            };

            return responseWrapper;
        }

        public static ResponseWrapper<TErrorCode> CreateUnsuccessfulResponse(TErrorCode validationErrorCode, IEnumerable<ValidationResult> validationResults)
        {
            var responseWrapper = new ResponseWrapper<TErrorCode>()
            {
                IsSuccessful = false,
                Errors = validationResults.Select(x => new Error<TErrorCode>(validationErrorCode, x.ErrorMessage)).ToList()
            };

            return responseWrapper;
        }


        [DataMember(Name = "isSuccessful")]
        public bool IsSuccessful { get; protected set; }

        [DataMember(Name = "hasContent")]
        public virtual bool HasContent { get; private set; } = false;

        [DataMember(Name = "errors")]
        public IReadOnlyList<Error<TErrorCode>> Errors { get; protected set; } = new List<Error<TErrorCode>>();

    }

    public class ResponseWrapper<TContent, TErrorCode>  : ResponseWrapper<TErrorCode>  where TErrorCode : struct, IConvertible
    {
        public static ResponseWrapper<TContent, TErrorCode> CreateSuccessfulResponse(TContent content)
        {
            var responseWrapper = new ResponseWrapper<TContent, TErrorCode>()
            {
                Content = content,
                IsSuccessful = true
            };

            return responseWrapper;
        }

        public new static ResponseWrapper<TContent, TErrorCode> CreateUnsuccessfulResponse(IEnumerable<Error<TErrorCode>> errors)
        {
            var responseWrapper = new ResponseWrapper<TContent, TErrorCode>()
            {
                IsSuccessful = false,
                Errors = errors.ToList()
            };

            return responseWrapper;
        }

        public new static ResponseWrapper<TContent, TErrorCode> CreateUnsuccessfulResponse(TErrorCode errorCode, string errorMessage)
        {
            var responseWrapper = new ResponseWrapper<TContent, TErrorCode>()
            {
                IsSuccessful = false,
                Errors = new List<Error<TErrorCode>>() { new Error<TErrorCode>(errorCode, errorMessage) }
            };

            return responseWrapper;
        }

        public new static ResponseWrapper<TContent, TErrorCode> CreateUnsuccessfulResponse(TErrorCode validationErrorCode, IEnumerable<ValidationResult> validationResults)
        {
            var responseWrapper = new ResponseWrapper<TContent, TErrorCode>()
            {
                IsSuccessful = false,
                Errors = validationResults.Select(x => new Error<TErrorCode>(validationErrorCode, x.ErrorMessage)).ToList()
            };

            return responseWrapper;
        }

        [DataMember(Name = "content")]
        public TContent Content { get; private set; }

        [DataMember(Name = "hasContent")]
        public override bool HasContent => Content != null;
    }
}