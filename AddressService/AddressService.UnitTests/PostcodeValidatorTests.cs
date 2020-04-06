using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Core.Services.PostcodeIo;
using AddressService.Core.Validation;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace AddressService.UnitTests
{
  public  class PostcodeValidatorTests
    {

        private  Mock<IPostcodeIoService> _postcodeIoService;
        private  Mock<IRepository> _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IRepository>();
            _postcodeIoService = new Mock<IPostcodeIoService>();
        }

        // Fails regex
        [TestCase(null,false,false,false)]
        [TestCase("",false,false,false)]
        [TestCase("  ",false,false,false)]
        [TestCase("NGG15FS",false,false,false)]
        [TestCase("NG15FSS",false,false,false)]
        [TestCase("NG15F",false,false,false)]


        // Passes regex
        [TestCase("NG15FS",true, true, true)]
        [TestCase("N15FS", true, true, true)]
        [TestCase("N11FS", true, true, true)]


        // Passes regex, but not in DB or determined as valid by PostcodeIO
        [TestCase("NG15ZZ", false, false, false)]

        // Passes regex and in DB
        [TestCase("NG15ZZ", true, false, true)]

        // Passes regex and not in DB, but is determined as valid by PostcodeIO
        [TestCase("NG15ZZ", false, true, true)]
        public async Task Test(string postcode, bool isPostcodeInDb,bool doesPostcodeIoSayPostcodeIsValid, bool expectedResult)
        {
            _repository.Setup(x => x.IsPostcodeInDb(It.IsAny<string>())).ReturnsAsync(isPostcodeInDb);
            _postcodeIoService.Setup(x => x.IsPostcodeValidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(doesPostcodeIoSayPostcodeIsValid);

            var postcodeValidator = new PostcodeValidator(_postcodeIoService.Object, _repository.Object);

            var result = await postcodeValidator.IsPostcodeValidAsync(postcode);

            Assert.AreEqual(expectedResult, result);
        }

    }
}
