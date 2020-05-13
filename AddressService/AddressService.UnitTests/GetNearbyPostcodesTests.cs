using AddressService.AzureFunction;
using AddressService.Core.Validation;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Utils;
using HelpMyStreet.Contracts.Shared;

namespace AddressService.UnitTests
{
    public class GetNearbyPostcodesTests
    {
        private Mock<IMediator>_mediator;
        private Mock<IPostcodeValidator> _postcodeValidator;
        private Mock<ILoggerWrapper<GetNearbyPostcodes>> _logger;
        private GetNearbyPostcodesResponse _response;

        [SetUp]
        public void SetUp()
        {
            _postcodeValidator = new Mock<IPostcodeValidator>();
            _postcodeValidator.Setup(x => x.IsPostcodeValidAsync(It.IsAny<string>())).ReturnsAsync(true);


            _response = new GetNearbyPostcodesResponse()
            {
                Postcodes = new List<GetNearbyPostCodeResponse>()
                {
                    new GetNearbyPostCodeResponse()
                    {
                        Postcode = "NG1 5FS",
                        AddressDetails = new List<AddressDetailsResponse>(),
                        DistanceInMetres = 2
                    }
                }
            };

            _mediator = new Mock<IMediator>();

            _mediator.Setup(x => x.Send(It.IsAny<GetNearbyPostcodesRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(_response);
            
            _logger = new Mock<ILoggerWrapper<GetNearbyPostcodes>>();
            _logger.SetupAllProperties();
        }


        [Test]
        public async Task HappyPath()
        {

            GetNearbyPostcodesRequest req = new GetNearbyPostcodesRequest(){
                Postcode = "NG1 5FS"
            };

            GetNearbyPostcodes getNearbyPostcodes = new GetNearbyPostcodes(_mediator.Object, _postcodeValidator.Object, _logger.Object);
            IActionResult result = await getNearbyPostcodes.Run(req, CancellationToken.None);

            OkObjectResult objectResult =   result as OkObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);

            ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);
            
            Assert.IsTrue(deserialisedResponse.HasContent);
            Assert.IsTrue(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(0,deserialisedResponse.Errors.Count());
            Assert.AreEqual("NG1 5FS", deserialisedResponse.Content.Postcodes.FirstOrDefault().Postcode);

            _mediator.Verify(x => x.Send(It.IsAny<GetNearbyPostcodesRequest>(), It.IsAny<CancellationToken>()));

        }

        [Test]
        public async Task InvalidPostcode()
        {
            _postcodeValidator.Setup(x => x.IsPostcodeValidAsync(It.IsAny<string>())).ReturnsAsync(false);

            GetNearbyPostcodesRequest req = new GetNearbyPostcodesRequest()
            {
                Postcode = "NG1 5FS"
            };

            GetNearbyPostcodes getNearbyPostcodes = new GetNearbyPostcodes(_mediator.Object, _postcodeValidator.Object, _logger.Object);
            IActionResult result = await getNearbyPostcodes.Run(req, CancellationToken.None);

            OkObjectResult objectResult = result as OkObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);
            
            ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);

            Assert.IsFalse(deserialisedResponse.HasContent);
            Assert.IsFalse(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(1, deserialisedResponse.Errors.Count());
            Assert.AreEqual(AddressServiceErrorCode.InvalidPostcode, deserialisedResponse.Errors[0].ErrorCode);

            _mediator.Verify(x => x.Send(It.IsAny<GetNearbyPostcodesRequest>(), It.IsAny<CancellationToken>()),Times.Never);
        }

        [Test]
        public async Task ErrorThrown()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetNearbyPostcodesRequest>(), It.IsAny<CancellationToken>())).Throws<Exception>();

            GetNearbyPostcodesRequest req = new GetNearbyPostcodesRequest()
            {
                Postcode = "NG1 5FS"
            };

            GetNearbyPostcodes getNearbyPostcodes = new GetNearbyPostcodes(_mediator.Object, _postcodeValidator.Object, _logger.Object);

            IActionResult result = await getNearbyPostcodes.Run(req, CancellationToken.None);

            ObjectResult objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);

            ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);
            Assert.AreEqual(500, objectResult.StatusCode); ;


            Assert.IsFalse(deserialisedResponse.HasContent);
            Assert.IsFalse(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(1, deserialisedResponse.Errors.Count());
            Assert.AreEqual(AddressServiceErrorCode.UnhandledError, deserialisedResponse.Errors[0].ErrorCode);

            _mediator.Verify(x => x.Send(It.IsAny<GetNearbyPostcodesRequest>(), It.IsAny<CancellationToken>()));

            _logger.Verify(x => x.LogErrorAndNotifyNewRelic(It.Is<string>(y => y.Contains("Unhandled error")), It.IsAny<Exception>(), It.IsAny<GetNearbyPostcodesRequest>()));
        }

        [Test]
        public async Task ValidationFails()
        {
            GetNearbyPostcodesRequest req = new GetNearbyPostcodesRequest()
            {
                Postcode = null
            };

            GetNearbyPostcodes getNearbyPostcodes = new GetNearbyPostcodes(_mediator.Object, _postcodeValidator.Object, _logger.Object);

            IActionResult result = await getNearbyPostcodes.Run(req, CancellationToken.None);

            ObjectResult objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);

            ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode> deserialisedResponse = objectResult.Value as ResponseWrapper<GetNearbyPostcodesResponse, AddressServiceErrorCode>;
            Assert.IsNotNull(deserialisedResponse);
            Assert.AreEqual(200, objectResult.StatusCode); ;


            Assert.IsFalse(deserialisedResponse.HasContent);
            Assert.IsFalse(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(1, deserialisedResponse.Errors.Count());
            Assert.AreEqual(AddressServiceErrorCode.ValidationError, deserialisedResponse.Errors[0].ErrorCode);

            _mediator.Verify(x => x.Send(It.IsAny<GetNearbyPostcodesRequest>(), It.IsAny<CancellationToken>()), Times.Never);

        }
    }
}
