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
using Newtonsoft.Json;

namespace AddressService.UnitTests
{
    public class IsPostcodeWithinRadiiTests
    {
        private Mock<IMediator> _mediator;
        private Mock<IPostcodeValidator> _postcodeValidator;
        private Mock<ILoggerWrapper<IsPostcodeWithinRadii>> _logger;
        private IsPostcodeWithinRadiiResponse _response;

        [SetUp]
        public void SetUp()
        {
            _postcodeValidator = new Mock<IPostcodeValidator>();
            _postcodeValidator.Setup(x => x.IsPostcodeValidAsync(It.IsAny<string>())).ReturnsAsync(true);


            _response = new IsPostcodeWithinRadiiResponse()
            {
                IdsWithinRadius = new List<int>() { 1 }
            };

            _mediator = new Mock<IMediator>();

            _mediator.Setup(x => x.Send(It.IsAny<IsPostcodeWithinRadiiRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(_response);

            _logger = new Mock<ILoggerWrapper<IsPostcodeWithinRadii>>();
            _logger.SetupAllProperties();
        }

        private HttpRequestMessage CreateRequest()
        {
            IsPostcodeWithinRadiiRequest req = new IsPostcodeWithinRadiiRequest()
            {
                Postcode = "NG1 5FS",
                PostcodeWithRadiuses = new List<PostcodeWithRadius>()
            };

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(req));
            return httpRequestMessage;
        }

        private HttpRequestMessage CreateInvalidRequest()
        {
            IsPostcodeWithinRadiiRequest req = new IsPostcodeWithinRadiiRequest()
            {
                Postcode = null,
                PostcodeWithRadiuses = new List<PostcodeWithRadius>()
            };

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(req));
            return httpRequestMessage;
        }

        [Test]
        public async Task HappyPath()
        {
            var req = CreateRequest();

            IsPostcodeWithinRadii isPostcodeWithinRadii = new IsPostcodeWithinRadii(_mediator.Object, _postcodeValidator.Object, _logger.Object);
            IActionResult result = await isPostcodeWithinRadii.Run(req, CancellationToken.None);

            OkObjectResult objectResult = result as OkObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);

            ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);

            Assert.IsTrue(deserialisedResponse.HasContent);
            Assert.IsTrue(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(0, deserialisedResponse.Errors.Count());
            Assert.AreEqual(1, deserialisedResponse.Content.IdsWithinRadius.FirstOrDefault());

            _mediator.Verify(x => x.Send(It.IsAny<IsPostcodeWithinRadiiRequest>(), It.IsAny<CancellationToken>()));

        }

        [Test]
        public async Task InvalidPostcode()
        {
            _postcodeValidator.Setup(x => x.IsPostcodeValidAsync(It.IsAny<string>())).ReturnsAsync(false);

            var req = CreateRequest();

            IsPostcodeWithinRadii isPostcodeWithinRadii = new IsPostcodeWithinRadii(_mediator.Object, _postcodeValidator.Object, _logger.Object);
            IActionResult result = await isPostcodeWithinRadii.Run(req, CancellationToken.None);

            OkObjectResult objectResult = result as OkObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);

            ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);

            Assert.IsFalse(deserialisedResponse.HasContent);
            Assert.IsFalse(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(1, deserialisedResponse.Errors.Count());
            Assert.AreEqual(AddressServiceErrorCode.InvalidPostcode, deserialisedResponse.Errors[0].ErrorCode);

            _mediator.Verify(x => x.Send(It.IsAny<IsPostcodeWithinRadiiRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task ErrorThrown()
        {
            _mediator.Setup(x => x.Send(It.IsAny<IsPostcodeWithinRadiiRequest>(), It.IsAny<CancellationToken>())).Throws<Exception>();

            var req = CreateRequest();

            IsPostcodeWithinRadii isPostcodeWithinRadii = new IsPostcodeWithinRadii(_mediator.Object, _postcodeValidator.Object, _logger.Object);

            IActionResult result = await isPostcodeWithinRadii.Run(req, CancellationToken.None);

            ObjectResult objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);

            ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);
            Assert.AreEqual(500, objectResult.StatusCode); ;


            Assert.IsFalse(deserialisedResponse.HasContent);
            Assert.IsFalse(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(1, deserialisedResponse.Errors.Count());
            Assert.AreEqual(AddressServiceErrorCode.UnhandledError, deserialisedResponse.Errors[0].ErrorCode);

            _mediator.Verify(x => x.Send(It.IsAny<IsPostcodeWithinRadiiRequest>(), It.IsAny<CancellationToken>()));

            _logger.Verify(x => x.LogError(It.Is<string>(y => y.Contains("Unhandled error in IsPostcodeWithinRadii")), It.IsAny<Exception>()));
        }

        [Test]
        public async Task ValidationFails()
        {
            var req = CreateInvalidRequest();

            IsPostcodeWithinRadii isPostcodeWithinRadii = new IsPostcodeWithinRadii(_mediator.Object, _postcodeValidator.Object, _logger.Object);

            IActionResult result = await isPostcodeWithinRadii.Run(req, CancellationToken.None);

            ObjectResult objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);

            ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);
            Assert.AreEqual(200, objectResult.StatusCode); ;


            Assert.IsFalse(deserialisedResponse.HasContent);
            Assert.IsFalse(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(1, deserialisedResponse.Errors.Count());
            Assert.AreEqual(AddressServiceErrorCode.ValidationError, deserialisedResponse.Errors[0].ErrorCode);

            _mediator.Verify(x => x.Send(It.IsAny<IsPostcodeWithinRadiiRequest>(), It.IsAny<CancellationToken>()), Times.Never);

        }
    }
}
