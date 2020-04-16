using AddressService.Core.Utils;
using AddressService.Core.Validation;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Contracts;
using AddressService.Core.Dto;
using AddressService.Core.Extensions;
using AddressService.Handlers;
using Marvin.StreamExtensions;
using Newtonsoft.Json;


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

        //public IsPostcodeWithinRadii(IMediator mediator, IPostcodeValidator postcodeValidator, ILoggerWrapper<IsPostcodeWithinRadii> logger)
        //{
        //    _mediator = mediator;
        //    _postcodeValidator = postcodeValidator;
        //    _logger = logger;
        //}

        [FunctionName("IsPostcodeWithinRadii")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage reqAsHttpRequestMessage,
            //[HttpTrigger(AuthorizationLevel.Function, "post", Route =null)] IsPostcodeWithinRadiiRequest req,
            CancellationToken cancellationToken)
        {
            ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode> result;
            try
            {

                //    var data = GetData().Result.Take(50000);


                //    var isPostcodeWithinRadiiRequest = new IsPostcodeWithinRadiiRequest()
                //    {
                //        Postcode = "NG1 5FS",
                //        PostcodeWithRadiuses = data,

                //    };

                //    //isPostcodeWithinRadiiRequest.CompressedPostcodeWithRadiuses = CompressionUtils.Zip(JsonConvert.SerializeObject(data));

                //    var json = JsonConvert.SerializeObject(isPostcodeWithinRadiiRequest);
                //    var compressedJson = CompressionUtils.Zip(json);


                //    var httpClient = new HttpClient();

                //    for (int i = 0; i < 10; i++)
                //    {


                //    var sw = new Stopwatch();
                //    sw.Start();
                //    var result = httpClient.PostAsync("http://localhost:7071/api/IsPostcodeWithinRadii", new ByteArrayContent(compressedJson)).Result;

                //    var responseString = result.Content.ReadAsStringAsync();
                //    sw.Stop();
                //    Debug.WriteLine(sw.ElapsedMilliseconds);
                //    }
                //}



                _logger.LogInformation("C# HTTP trigger function processed a request.");

                var sw = new Stopwatch();
                sw.Start();
                // The list in the request doesn't get deserialised when accepting IsPostcodeWithinRadiiRequest in the method...
                var sw2 = new Stopwatch();
                sw2.Start();

                IsPostcodeWithinRadiiRequest req;

                if (reqAsHttpRequestMessage.Content.Headers.ContentEncoding.Any(x => x.ToLower() == "gzip"))
                {
                    var inputStream = await reqAsHttpRequestMessage.Content.ReadAsStreamAsync();
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


                // return new OkObjectResult("hello");
                //Stream stream = await reqAsHttpRequestMessage.Content.ReadAsStreamAsync();
                //IsPostcodeWithinRadiiRequest req = stream.ReadAndDeserializeFromJson<IsPostcodeWithinRadiiRequest>();

                //Stream stream = await reqAsHttpRequestMessage.Content.ReadAsStreamAsync();
                //IsPostcodeWithinRadiiRequest req = await Utf8Json.JsonSerializer.DeserializeAsync<IsPostcodeWithinRadiiRequest>(stream);

                //var aaa = await reqAsHttpRequestMessage.Content.ReadAsByteArrayAsync();
                //var decompresed = CompressionUtils.Unzip(aaa);

                //IsPostcodeWithinRadiiRequest req = JsonConvert.DeserializeObject<IsPostcodeWithinRadiiRequest>(decompresed);

                sw2.Stop();
                Debug.WriteLine($"deserialisation: {sw2.ElapsedMilliseconds}");

                //This validation logic belongs in a custom validation attribute on the Postcode property.  However, validationContext.GetService<IExternalService> always returned null in the validation attribute(despite DI working fine elsewhere).I didn't want to spend a lot of time finding out why when there is lots to do so I've put the postcode validation logic here for now.


                if (!await _postcodeValidator.IsPostcodeValidAsync(req.Postcode))
                {
                    result = ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.InvalidPostcode, "Invalid postcode");

                    //return new OkObjectResult(ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.InvalidPostcode, "Invalid postcode"));
                }

                if (req.IsValid(out var validationResults))
                {


                    var sw3 = new Stopwatch();
                    sw3.Start();

                    IsPostcodeWithinRadiiResponse response = await _mediator.Send(req, cancellationToken);

                    // IsPostcodeWithinRadiiResponse response = await _isPostcodeWithinRadiiHandler.Handle(req, cancellationToken);

                    sw3.Stop();
                    Debug.WriteLine($"_isPostcodeWithinRadiiHandler.Handle(req, cancellationToken): {sw3.ElapsedMilliseconds}");

                    sw.Stop();
                    Debug.WriteLine($"full request: {sw.ElapsedMilliseconds}");

                    //var json = JsonConvert.SerializeObject(ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.ValidationError, validationResults), Formatting.None);
                    //return new HttpResponseMessage(HttpStatusCode.OK)
                    //{
                    //    Content = new StringContent(json, Encoding.UTF8, "application/json")
                    //};

                    result = ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>.CreateSuccessfulResponse(response);
                    /// return new OkObjectResult(ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>.CreateSuccessfulResponse(response));


                }
                else
                {
                    result = ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.ValidationError, validationResults);
                    // return new OkObjectResult(ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.ValidationError, validationResults));
                }

                //var json = JsonConvert.SerializeObject(result, Formatting.None);
                var json = Utf8Json.JsonSerializer.Serialize(result);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new ByteArrayContent(json)
                };

            }
            catch (Exception ex)
            {
                _logger.LogError("Unhandled error in GetNearbyPostcodes", ex);

                result = ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.UnhandledError, "Internal Error");

                var json = JsonConvert.SerializeObject(result, Formatting.None);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                //  return new ObjectResult(ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}

