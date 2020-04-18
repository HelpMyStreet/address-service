using AddressService.Core.Utils;
using AddressService.Core.Validation;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.AddressService.Request;


namespace AddressService.AzureFunction
{
    public class IsPostcodeWithinRadii
    {
        private readonly IMediator _mediator;
        private readonly IPostcodeValidator _postcodeValidator;
        private readonly ILoggerWrapper<IsPostcodeWithinRadii> _logger;

        public IsPostcodeWithinRadii(IMediator mediator, IPostcodeValidator postcodeValidator, ILoggerWrapper<IsPostcodeWithinRadii> logger)
        {
            _mediator = mediator;
            _postcodeValidator = postcodeValidator;
            _logger = logger;
        }

        [FunctionName("IsPostcodeWithinRadii")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage reqAsHttpRequestMessage,
            CancellationToken cancellationToken)
        {
            ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode> result;
            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a request.");

                IsPostcodeWithinRadiiRequest req;

                // Allow requests to be compressed and use a faster deserialiser than the default
                if (reqAsHttpRequestMessage.Content.Headers.ContentEncoding.Any(x => x.ToLower() == "gzip"))
                {
                    Stream inputStream = await reqAsHttpRequestMessage.Content.ReadAsStreamAsync();
                    using (GZipStream decompressionStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        req = await Utf8Json.JsonSerializer.DeserializeAsync<IsPostcodeWithinRadiiRequest>(decompressionStream);
                    }
                }
                else
                {
                    Stream stream = await reqAsHttpRequestMessage.Content.ReadAsStreamAsync();
                    req = await Utf8Json.JsonSerializer.DeserializeAsync<IsPostcodeWithinRadiiRequest>(stream);
                }

                //This validation logic belongs in a custom validation attribute on the Postcode property.  However, validationContext.GetService<IExternalService> always returned null in the validation attribute (despite DI working fine elsewhere). I didn't want to spend a lot of time finding out why when there is lots to do so I've put the postcode validation logic here for now.
                if (!await _postcodeValidator.IsPostcodeValidAsync(req.Postcode))
                {
                    result = ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.InvalidPostcode, "Invalid postcode");
                }

                if (req.IsValid(out var validationResults))
                {
                    IsPostcodeWithinRadiiResponse response = await _mediator.Send(req, cancellationToken);

                    return new OkObjectResult(ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>.CreateSuccessfulResponse(response));
                }
                else
                {
                    return new OkObjectResult(ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.ValidationError, validationResults));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Unhandled error in IsPostcodeWithinRadii", ex);
                return new ObjectResult(ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}

