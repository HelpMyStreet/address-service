using AddressService.Core.Services.PostcodeIo;
using AddressService.Core.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Interfaces.Repositories;

namespace AddressService.UnitTests
{
    public class PostcodeValidatorTests
    {
        private Mock<IRegexPostcodeValidator> _regexPostcodeValidator;
        private Mock<IRepository> _repository;
        private Mock<ILogger<PostcodeValidator>> _iLogger;

        [SetUp]
        public void SetUp()
        {
            _regexPostcodeValidator = new Mock<IRegexPostcodeValidator>();
            _repository = new Mock<IRepository>();
            _iLogger = new Mock<ILogger<PostcodeValidator>>();
            _iLogger.SetupAllProperties();
        }

        [TestCase(false, true, false, Description = "Fails regex")]
        [TestCase(true, true, true, Description = "Passes regex")]
        [TestCase(true, false, false, Description = "Passes regex, but not marked as active in DB")]
        [TestCase(true, true, true, Description = "Passes regex and is marked as active in DB")]
        public async Task IsPostcodeValid(bool passesRegexValidator, bool isInDbAndActive, bool expectedResult)
        {
            _regexPostcodeValidator.Setup(x => x.IsPostcodeValid(It.IsAny<string>())).Returns(passesRegexValidator);
            _repository.Setup(x => x.IsPostcodeInDbAndActive(It.IsAny<string>())).ReturnsAsync(isInDbAndActive);

            PostcodeValidator postcodeValidator = new PostcodeValidator(_regexPostcodeValidator.Object, _repository.Object, _iLogger.Object);

            bool result = await postcodeValidator.IsPostcodeValidAsync("NG15FS");

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task PassesRegexButPostcodeIOThrowsError()
        {
            _regexPostcodeValidator.Setup(x => x.IsPostcodeValid(It.IsAny<string>())).Returns(true);
            _repository.Setup(x => x.IsPostcodeInDbAndActive(It.IsAny<string>())).Throws<Exception>();

            PostcodeValidator postcodeValidator = new PostcodeValidator(_regexPostcodeValidator.Object, _repository.Object, _iLogger.Object);

            bool result = await postcodeValidator.IsPostcodeValidAsync("NG15FS");

            Assert.IsTrue(result);
        }

    }
}
