using AddressService.Core.Services.PostcodeIo;
using AddressService.Core.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.UnitTests
{
    public class PostcodeValidatorTests
    {
        private Mock<IRegexPostcodeValidator> _regexPostcodeValidator;
        private Mock<IPostcodeIoService> _postcodeIoService;
        private Mock<ILogger<PostcodeValidator>> _iLogger;

        [SetUp]
        public void SetUp()
        {
            _regexPostcodeValidator = new Mock<IRegexPostcodeValidator>();
            _postcodeIoService = new Mock<IPostcodeIoService>();
            _iLogger = new Mock<ILogger<PostcodeValidator>>();
            _iLogger.SetupAllProperties();
        }

        [TestCase(false, true, false, Description = "Fails regex")]
        [TestCase(true, true, true, Description = "Passes regex")]
        [TestCase(true, false, false, Description = "Passes regex, but not determined as valid by PostcodeIO")]
        [TestCase(true, true, true, Description = "Passes regex and is determined as valid by PostcodeIO")]
        public async Task IsPostcodeValid(bool passesRegexValidator, bool doesPostcodeIoSayPostcodeIsValid, bool expectedResult)
        {
            _regexPostcodeValidator.Setup(x => x.IsPostcodeValid(It.IsAny<string>())).Returns(passesRegexValidator);
            _postcodeIoService.Setup(x => x.IsPostcodeValidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(doesPostcodeIoSayPostcodeIsValid);

            PostcodeValidator postcodeValidator = new PostcodeValidator(_regexPostcodeValidator.Object, _postcodeIoService.Object, _iLogger.Object);

            bool result = await postcodeValidator.IsPostcodeValidAsync("NG15FS");

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task PassesRegexButPostcodeIOThrowsError()
        {
            _regexPostcodeValidator.Setup(x => x.IsPostcodeValid(It.IsAny<string>())).Returns(true);
            _postcodeIoService.Setup(x => x.IsPostcodeValidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws<Exception>();

            PostcodeValidator postcodeValidator = new PostcodeValidator(_regexPostcodeValidator.Object, _postcodeIoService.Object, _iLogger.Object);

            bool result = await postcodeValidator.IsPostcodeValidAsync("NG15FS");

            Assert.IsTrue(result);
        }

    }
}
