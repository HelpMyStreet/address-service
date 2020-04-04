using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace AddressService.Core.Utils
{
    public class ResponseWrapper
    {
        public static ResponseWrapper CreateSuccessfulResponse()
        {
            var responseWrapper = new ResponseWrapper()
            {
                IsSuccessful = true
            };

            return responseWrapper;
        }

        public static ResponseWrapper CreateUnsuccessfulResponse(IEnumerable<string> errors)
        {
            var responseWrapper = new ResponseWrapper()
            {
                IsSuccessful = false,
                Errors = errors
            };

            return responseWrapper;
        }

        public static ResponseWrapper CreateUnsuccessfulResponse(string errorMessage)
        {
            var responseWrapper = new ResponseWrapper()
            {
                IsSuccessful = false,
                Errors = new List<string>() { errorMessage }
            };

            return responseWrapper;
        }

        public static ResponseWrapper CreateUnsuccessfulResponse(IEnumerable<ValidationResult> validationResults)
        {
            var responseWrapper = new ResponseWrapper()
            {
                IsSuccessful = false,
                Errors = validationResults.Select(x => x.ErrorMessage)
            };

            return responseWrapper;
        }


        [DataMember(Name = "isSuccessful")]
        public bool IsSuccessful { get; protected set; }

        [DataMember(Name = "hasContent")]
        public virtual bool HasContent { get; private set; } = false;

        [DataMember(Name = "errors")]
        public IEnumerable<string> Errors { get; protected set; } = new List<string>();

    }

    public class ResponseWrapper<T> : ResponseWrapper
    {
        public static ResponseWrapper CreateSuccessfulResponse(T content)
        {
            var responseWrapper = new ResponseWrapper<T>()
            {
                Content = content,
                IsSuccessful = true
            };

            return responseWrapper;
        }

        public new static ResponseWrapper CreateUnsuccessfulResponse(string errorMessage)
        {
            var responseWrapper = new ResponseWrapper<T>()
            {
                IsSuccessful = false,
                Errors = new List<string>() { errorMessage }
            };

            return responseWrapper;
        }

        public new static ResponseWrapper CreateUnsuccessfulResponse(IEnumerable<ValidationResult> validationResults)
        {
            var responseWrapper = new ResponseWrapper<T>()
            {
                IsSuccessful = false,
                Errors = validationResults.Select(x => x.ErrorMessage)
            };

            return responseWrapper;
        }


        [DataMember(Name = "content")]
        public T Content { get; private set; }

        [DataMember(Name = "hasContent")]
        public override bool HasContent => Content != null;
    }
}