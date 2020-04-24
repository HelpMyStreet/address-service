using AddressService.AzureFunction;
using AddressService.Core.Utils;
using AddressService.Core.Validation;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Contracts;
using Newtonsoft.Json;

namespace AddressService.UnitTests
{
    public class GetPostcodeCoordinatesTests
    {
        private Mock<IMediator> _mediator;
        private Mock<IPostcodeValidator> _postcodeValidator;
        private Mock<ILoggerWrapper<GetPostcodeCoordinates>> _loggerWrapper;
        private GetPostcodeCoordinatesResponse _response;

        [SetUp]
        public void SetUp()
        {
            _postcodeValidator = new Mock<IPostcodeValidator>();
            _postcodeValidator.Setup(x => x.IsPostcodeValidAsync(It.IsAny<string>())).ReturnsAsync(true);

            _response = new GetPostcodeCoordinatesResponse()
            {
                PostcodeCoordinates = new List<PostcodeCoordinate>()
               {
                   new PostcodeCoordinate()
                   {
                       Postcode = "NG1 5FS",
                       Latitude = 52.954885,
                       Longitude = -1.155263
                   }
               }
            };

            _mediator = new Mock<IMediator>();

            _mediator.Setup(x => x.Send(It.IsAny<GetPostcodeCoordinatesRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(_response);

            _loggerWrapper = new Mock<ILoggerWrapper<GetPostcodeCoordinates>>();
            _loggerWrapper.SetupAllProperties();
        }

        private HttpRequestMessage CreateRequest()
        {
            GetPostcodeCoordinatesRequest req = new GetPostcodeCoordinatesRequest()
            {
                Postcodes = new List<string>()
                {
                    "NG1 5FS"
                }
            };

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(req));
            return httpRequestMessage;
        }

        private HttpRequestMessage CreateInvalidRequest()
        {
            GetPostcodeCoordinatesRequest req = new GetPostcodeCoordinatesRequest()
            {
                Postcodes = null
            };

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(req));
            return httpRequestMessage;
        }

        [Test]
        public async Task HappyPath()
        {
            var req = CreateRequest();

            GetPostcodeCoordinates getPostcodeCoordinates = new GetPostcodeCoordinates(_mediator.Object, _loggerWrapper.Object);
            IActionResult result = await getPostcodeCoordinates.Run(req, CancellationToken.None);

            OkObjectResult objectResult = result as OkObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);

            ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);

            Assert.IsTrue(deserialisedResponse.HasContent);
            Assert.IsTrue(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(0, deserialisedResponse.Errors.Count());
            Assert.AreEqual(1, deserialisedResponse.Content.PostcodeCoordinates.Count());

            _mediator.Verify(x => x.Send(It.IsAny<GetPostcodeCoordinatesRequest>(), It.IsAny<CancellationToken>()));

        }

        [Test]
        public async Task ErrorThrown()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetPostcodeCoordinatesRequest>(), It.IsAny<CancellationToken>())).Throws<Exception>();

            var req = CreateRequest();

            GetPostcodeCoordinates getPostcodeCoordinates = new GetPostcodeCoordinates(_mediator.Object, _loggerWrapper.Object);

            IActionResult result = await getPostcodeCoordinates.Run(req, CancellationToken.None);

            ObjectResult objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);

            ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);
            Assert.AreEqual(500, objectResult.StatusCode); ;


            Assert.IsFalse(deserialisedResponse.HasContent);
            Assert.IsFalse(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(1, deserialisedResponse.Errors.Count());
            Assert.AreEqual(AddressServiceErrorCode.UnhandledError, deserialisedResponse.Errors[0].ErrorCode);

            _mediator.Verify(x => x.Send(It.IsAny<GetPostcodeCoordinatesRequest>(), It.IsAny<CancellationToken>()));

            _loggerWrapper.Verify(x => x.LogError(It.Is<string>(y => y.Contains("Unhandled error in GetPostcodeCoordinates")), It.IsAny<Exception>()));
        }

        [Test]
        public async Task ValidationFails()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetPostcodeCoordinatesRequest>(), It.IsAny<CancellationToken>())).Throws<Exception>();

            var req = CreateInvalidRequest();

            GetPostcodeCoordinates getPostcodeCoordinates = new GetPostcodeCoordinates(_mediator.Object, _loggerWrapper.Object);

            IActionResult result = await getPostcodeCoordinates.Run(req, CancellationToken.None);

            ObjectResult objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);

            ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);
            Assert.AreEqual(200, objectResult.StatusCode); ;


            Assert.IsFalse(deserialisedResponse.HasContent);
            Assert.IsFalse(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(1, deserialisedResponse.Errors.Count());
            Assert.AreEqual(AddressServiceErrorCode.ValidationError, deserialisedResponse.Errors[0].ErrorCode);

            _mediator.Verify(x => x.Send(It.IsAny<GetPostcodeCoordinatesRequest>(), It.IsAny<CancellationToken>()), Times.Never);

        }
    }
}
