﻿using AddressService.AzureFunction;
using AddressService.Core.Utils;
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

namespace AddressService.UnitTests
{
    public class GetPostcodeTests
    {
        private Mock<IMediator>_mediator;
        private Mock<IPostcodeValidator> _postcodeValidator;
        private Mock<ILogger> _logger;
        private GetPostcodeResponse _response;

        [SetUp]
        public void SetUp()
        {
            _postcodeValidator = new Mock<IPostcodeValidator>();
            _postcodeValidator.Setup(x => x.IsPostcodeValidAsync(It.IsAny<string>())).ReturnsAsync(true);


            _response = new GetPostcodeResponse()
            {
                AddressDetails = new List<AddressDetailsResponse>(),
                Postcode = "NG1 5FS"
            };

            _mediator = new Mock<IMediator>();

            _mediator.Setup(x => x.Send(It.IsAny<GetPostcodeRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(_response);
            
            _logger = new Mock<ILogger>();
            _logger.SetupAllProperties();
        }


        [Test]
        public async Task HappyPath()
        {
           
            GetPostcodeRequest req = new GetPostcodeRequest(){
                Postcode = "NG1 5FS"
            };

            var getPostcode = new GetPostcode(_mediator.Object, _postcodeValidator.Object);
            var result = await getPostcode.Run(req, CancellationToken.None, _logger.Object);

            var objectResult =   result as OkObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);

            var deserialisedResponse = objectResult.Value as ResponseWrapper<GetPostcodeResponse>;
            Assert.IsNotNull(deserialisedResponse);
            
            Assert.IsTrue(deserialisedResponse.HasContent);
            Assert.IsTrue(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(0,deserialisedResponse.Errors.Count());
            Assert.AreEqual("NG1 5FS", deserialisedResponse.Content.Postcode);

            _mediator.Verify(x => x.Send(It.IsAny<GetPostcodeRequest>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task InvalidPostcode()
        {
            _postcodeValidator.Setup(x => x.IsPostcodeValidAsync(It.IsAny<string>())).ReturnsAsync(false);

            GetPostcodeRequest req = new GetPostcodeRequest()
            {
                Postcode = "NG1 5FS"
            };

            var getPostcode = new GetPostcode(_mediator.Object, _postcodeValidator.Object);
            var result = await getPostcode.Run(req, CancellationToken.None, _logger.Object);

            var objectResult = result as OkObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);
            
            var deserialisedResponse = objectResult.Value as ResponseWrapper<GetPostcodeResponse>;
            Assert.IsNotNull(deserialisedResponse);

            Assert.IsFalse(deserialisedResponse.HasContent);
            Assert.IsFalse(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(1, deserialisedResponse.Errors.Count());;

            _mediator.Verify(x => x.Send(It.IsAny<GetPostcodeRequest>(), It.IsAny<CancellationToken>()),Times.Never);
        }

        [Test]
        public async Task ErrorThrown()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetPostcodeRequest>(), It.IsAny<CancellationToken>())).Throws<Exception>();

            GetPostcodeRequest req = new GetPostcodeRequest()
            {
                Postcode = "NG1 5FS"
            };

            var getPostcode = new GetPostcode(_mediator.Object, _postcodeValidator.Object);
            var result = await getPostcode.Run(req, CancellationToken.None, _logger.Object);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);

            var deserialisedResponse = objectResult.Value as ResponseWrapper<GetPostcodeResponse>;
            Assert.IsNotNull(deserialisedResponse);
            Assert.AreEqual(500, objectResult.StatusCode); ;


            Assert.IsFalse(deserialisedResponse.HasContent);
            Assert.IsFalse(deserialisedResponse.IsSuccessful);
            Assert.AreEqual(1, deserialisedResponse.Errors.Count()); ;

            _mediator.Verify(x => x.Send(It.IsAny<GetPostcodeRequest>(), It.IsAny<CancellationToken>()));
        }
    }
}
