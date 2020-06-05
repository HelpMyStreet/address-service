using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AddressService.AzureFunction;
using AddressService.Core.Utils;
using AddressService.Core.Validation;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace AddressService.UnitTests.AzureFunctions
{
    public class GetNearbyPostcodesWithoutAddressesTests
    {
        private Mock<IMediator> _mediator;
        private Mock<IPostcodeValidator> _postcodeValidator;
        private Mock<ILoggerWrapper<GetNearbyPostcodesWithoutAddresses>> _logger;
        private GetNearbyPostcodesWithoutAddressesResponse _response;

        [SetUp]
        public void SetUp()
        {
            _postcodeValidator = new Mock<IPostcodeValidator>();
            _postcodeValidator.Setup(x => x.IsPostcodeValidAsync(It.IsAny<string>())).ReturnsAsync(true);


            _response = new GetNearbyPostcodesWithoutAddressesResponse()
            {
                NearestPostcodes = new List<NearestPostcodeWithoutAddress>()
               {
                   new NearestPostcodeWithoutAddress()
                   {
                       Postcode ="NG1 5FS",
                       DistanceInMetres = 1
                   }
               }
            };

            _mediator = new Mock<IMediator>();

            _mediator.Setup(x => x.Send(It.IsAny<GetNearbyPostcodesWithoutAddressesRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(_response);

            _logger = new Mock<ILoggerWrapper<GetNearbyPostcodesWithoutAddresses>>();
            _logger.SetupAllProperties();
        }


        [Test]
        public async Task HappyPath()
        {

            GetNearbyPostcodesWithoutAddressesRequest req = new GetNearbyPostcodesWithoutAddressesRequest()
            {
                Postcode = "NG1 5FS"
            };

            GetNearbyPostcodesWithoutAddresses getNearbyPostcodes = new GetNearbyPostcodesWithoutAddresses(_mediator.Object, _postcodeValidator.Object, _logger.Object);
            IActionResult result = await getNearbyPostcodes.Run(req, CancellationToken.None);

            OkObjectResult objectResult = result as OkObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);

            ResponseWrapper<GetNearbyPostcodesWithoutAddressesResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<GetNearbyPostcodesWithoutAddressesResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);

            Assert.IsTrue(deserialisedResponse.HasContent);
            Assert.IsTrue(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(0, deserialisedResponse.Errors.Count());
            Assert.AreEqual("NG1 5FS", deserialisedResponse.Content.NearestPostcodes.FirstOrDefault().Postcode);

            _mediator.Verify(x => x.Send(It.IsAny<GetNearbyPostcodesWithoutAddressesRequest>(), It.IsAny<CancellationToken>()));

        }

        [Test]
        public async Task InvalidPostcode()
        {
            _postcodeValidator.Setup(x => x.IsPostcodeValidAsync(It.IsAny<string>())).ReturnsAsync(false);

            GetNearbyPostcodesWithoutAddressesRequest req = new GetNearbyPostcodesWithoutAddressesRequest()
            {
                Postcode = "NG1 5FS"
            };

            GetNearbyPostcodesWithoutAddresses getNearbyPostcodes = new GetNearbyPostcodesWithoutAddresses(_mediator.Object, _postcodeValidator.Object, _logger.Object);
            IActionResult result = await getNearbyPostcodes.Run(req, CancellationToken.None);

            ObjectResult objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(422, objectResult.StatusCode);

            ResponseWrapper<GetNearbyPostcodesWithoutAddressesResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<GetNearbyPostcodesWithoutAddressesResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);

            Assert.IsFalse(deserialisedResponse.HasContent);
            Assert.IsFalse(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(1, deserialisedResponse.Errors.Count());
            Assert.AreEqual(AddressServiceErrorCode.InvalidPostcode, deserialisedResponse.Errors[0].ErrorCode);

            _mediator.Verify(x => x.Send(It.IsAny<GetNearbyPostcodesWithoutAddressesRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task ErrorThrown()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetNearbyPostcodesWithoutAddressesRequest>(), It.IsAny<CancellationToken>())).Throws<Exception>();

            GetNearbyPostcodesWithoutAddressesRequest req = new GetNearbyPostcodesWithoutAddressesRequest()
            {
                Postcode = "NG1 5FS"
            };

            GetNearbyPostcodesWithoutAddresses getNearbyPostcodes = new GetNearbyPostcodesWithoutAddresses(_mediator.Object, _postcodeValidator.Object, _logger.Object);

            IActionResult result = await getNearbyPostcodes.Run(req, CancellationToken.None);

            ObjectResult objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);

            ResponseWrapper<GetNearbyPostcodesWithoutAddressesResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<GetNearbyPostcodesWithoutAddressesResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);
            Assert.AreEqual(500, objectResult.StatusCode); ;


            Assert.IsFalse(deserialisedResponse.HasContent);
            Assert.IsFalse(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(1, deserialisedResponse.Errors.Count());
            Assert.AreEqual(AddressServiceErrorCode.UnhandledError, deserialisedResponse.Errors[0].ErrorCode);

            _mediator.Verify(x => x.Send(It.IsAny<GetNearbyPostcodesWithoutAddressesRequest>(), It.IsAny<CancellationToken>()));

            _logger.Verify(x => x.LogErrorAndNotifyNewRelic(It.Is<string>(y => y.Contains("Unhandled error")), It.IsAny<Exception>(), It.IsAny<GetNearbyPostcodesWithoutAddressesRequest>()));
        }
    }
}
