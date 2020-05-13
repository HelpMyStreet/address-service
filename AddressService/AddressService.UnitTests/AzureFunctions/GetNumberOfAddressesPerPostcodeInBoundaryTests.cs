using AddressService.AzureFunction;
using AddressService.Core.Utils;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.UnitTests.AzureFunctions
{
    public class GetNumberOfAddressesPerPostcodeInBoundaryTests
    {
        private Mock<IMediator> _mediator;
        private Mock<ILoggerWrapper<GetNumberOfAddressesPerPostcodeInBoundary>> _logger;
        private GetNumberOfAddressesPerPostcodeInBoundaryResponse _response;

        [SetUp]
        public void SetUp()
        {
            _response = new GetNumberOfAddressesPerPostcodeInBoundaryResponse()
            {
                PostcodesWithNumberOfAddresses = new List<PostcodeWithNumberOfAddresses>()
                {
                    new PostcodeWithNumberOfAddresses()
                    {
                        NumberOfAddresses = 12,
                        Postcode = "NG1 1AA"
                    }
                }
            };

            _mediator = new Mock<IMediator>();

            _mediator.Setup(x => x.Send(It.IsAny<GetNumberOfAddressesPerPostcodeInBoundaryRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(_response);

            _logger = new Mock<ILoggerWrapper<GetNumberOfAddressesPerPostcodeInBoundary>>();
            _logger.SetupAllProperties();
        }


        [Test]
        public async Task HappyPath()
        {
            GetNumberOfAddressesPerPostcodeInBoundaryRequest req = new GetNumberOfAddressesPerPostcodeInBoundaryRequest()
            {
                SwLatitude = 1,
                SwLongitude = 2,
                NeLatitude = 3,
                NeLongitude = 4
            };

            GetNumberOfAddressesPerPostcodeInBoundary GetNumberOfAddressesPerPostcodeInBoundary = new GetNumberOfAddressesPerPostcodeInBoundary(_mediator.Object, _logger.Object);
            IActionResult result = await GetNumberOfAddressesPerPostcodeInBoundary.Run(req, CancellationToken.None);

            OkObjectResult objectResult = result as OkObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);

            ResponseWrapper<GetNumberOfAddressesPerPostcodeInBoundaryResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<GetNumberOfAddressesPerPostcodeInBoundaryResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);

            Assert.IsTrue(deserialisedResponse.HasContent);
            Assert.IsTrue(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(0, deserialisedResponse.Errors.Count());
            Assert.AreEqual("NG1 1AA", deserialisedResponse.Content.PostcodesWithNumberOfAddresses.FirstOrDefault().Postcode);

            _mediator.Verify(x => x.Send(It.IsAny<GetNumberOfAddressesPerPostcodeInBoundaryRequest>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ErrorThrown()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetNumberOfAddressesPerPostcodeInBoundaryRequest>(), It.IsAny<CancellationToken>())).Throws<Exception>();
            GetNumberOfAddressesPerPostcodeInBoundaryRequest req = new GetNumberOfAddressesPerPostcodeInBoundaryRequest()
            {
                SwLatitude = 1,
                SwLongitude = 2,
                NeLatitude = 3,
                NeLongitude = 4
            };

            GetNumberOfAddressesPerPostcodeInBoundary GetNumberOfAddressesPerPostcodeInBoundary = new GetNumberOfAddressesPerPostcodeInBoundary(_mediator.Object, _logger.Object);
            IActionResult result = await GetNumberOfAddressesPerPostcodeInBoundary.Run(req, CancellationToken.None);

            ObjectResult objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);

            ResponseWrapper<GetNumberOfAddressesPerPostcodeInBoundaryResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<GetNumberOfAddressesPerPostcodeInBoundaryResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);
            Assert.AreEqual(500, objectResult.StatusCode); ;


            Assert.IsFalse(deserialisedResponse.HasContent);
            Assert.IsFalse(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(1, deserialisedResponse.Errors.Count());
            Assert.AreEqual(AddressServiceErrorCode.UnhandledError, deserialisedResponse.Errors[0].ErrorCode);

            _mediator.Verify(x => x.Send(It.IsAny<GetNumberOfAddressesPerPostcodeInBoundaryRequest>(), It.IsAny<CancellationToken>()));

            _logger.Verify(x => x.LogError(It.Is<string>(y => y.Contains("Unhandled error in GetNumberOfAddressesPerPostcodeInBoundary")), It.IsAny<Exception>()));
        }

        [Test]
        public async Task ValidationFails()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetNumberOfAddressesPerPostcodeInBoundaryRequest>(), It.IsAny<CancellationToken>())).Throws<Exception>();

            GetNumberOfAddressesPerPostcodeInBoundaryRequest req = new GetNumberOfAddressesPerPostcodeInBoundaryRequest()
            {
                SwLatitude = 999, // invalid
                SwLongitude = 2,
                NeLatitude = 3,
                NeLongitude = 4
            };

            GetNumberOfAddressesPerPostcodeInBoundary GetNumberOfAddressesPerPostcodeInBoundary = new GetNumberOfAddressesPerPostcodeInBoundary(_mediator.Object, _logger.Object);

            IActionResult result = await GetNumberOfAddressesPerPostcodeInBoundary.Run(req, CancellationToken.None);

            ObjectResult objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);

            ResponseWrapper<GetNumberOfAddressesPerPostcodeInBoundaryResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<GetNumberOfAddressesPerPostcodeInBoundaryResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);
            Assert.AreEqual(422, objectResult.StatusCode); ;


            Assert.IsFalse(deserialisedResponse.HasContent);
            Assert.IsFalse(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(1, deserialisedResponse.Errors.Count());
            Assert.AreEqual(AddressServiceErrorCode.ValidationError, deserialisedResponse.Errors[0].ErrorCode);

            _mediator.Verify(x => x.Send(It.IsAny<GetNumberOfAddressesPerPostcodeInBoundaryRequest>(), It.IsAny<CancellationToken>()), Times.Never);

        }
    }
}
