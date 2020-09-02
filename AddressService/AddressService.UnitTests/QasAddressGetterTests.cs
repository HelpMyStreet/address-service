using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Core.Services.Qas;
using AddressService.Core.Utils;
using AddressService.Handlers;
using AddressService.Mappers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Handlers.BusinessLogic;
using HelpMyStreet.Utils.Utils;

namespace AddressService.UnitTests
{
    public class QasAddressGetterTests
    {
        private Mock<IRepository> _repository;
        private Mock<IQasService> _qasService;
        private Mock<IQasMapper> _qasMapper;
        private Mock<IFriendlyNameGenerator> _friendlyNameGenerator;
        private Mock<ILoggerWrapper<QasAddressGetter>> _logger;

        private PostcodeDto _missingPostcodeDtosFromQas;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IRepository>();

           

            _repository.SetupAllProperties();


            _qasService = new Mock<IQasService>();

            _qasService.SetupAllProperties();

            _qasMapper = new Mock<IQasMapper>();

            ILookup<string, string> missingQasFormatIdsGroupedByPostCode = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("ng15ba", "AC4705C7-E358-4338-A4FC-DE5EA241B5F1")
            }.ToLookup(x => x.Key, x => x.Value);

            _qasMapper.Setup(x => x.GetFormatIds(It.IsAny<IEnumerable<QasSearchRootResponse>>())).Returns(missingQasFormatIdsGroupedByPostCode);

            _missingPostcodeDtosFromQas = new PostcodeDto()
            {
                Postcode = "NG1 6DQ",
                AddressDetails = new List<AddressDetailsDto>()
                {
                    new AddressDetailsDto()
                    {
                        AddressLine1 = "2_addressline1",
                        AddressLine2 = "2_addressline2",
                        AddressLine3 = "2_addressline1",
                        Locality = "2_locality"
                    }
                }
            };

            _qasMapper.Setup(x => x.MapToPostcodeDto(It.IsAny<string>(), It.IsAny<IEnumerable<QasFormatRootResponse>>())).Returns(_missingPostcodeDtosFromQas);

            _friendlyNameGenerator = new Mock<IFriendlyNameGenerator>();
            _friendlyNameGenerator.Setup(x => x.GenerateFriendlyName(It.IsAny<PostcodeDto>()));

            _logger = new Mock<ILoggerWrapper<QasAddressGetter>>();

            _logger.SetupAllProperties();
        }


        [Test]
        public async Task MissingPostCodeIsRetrievedFromQas()
        {
            CancellationToken cancellationToken = new CancellationToken();
            QasAddressGetter qasAddressGetter = new QasAddressGetter(_repository.Object, _qasService.Object, _qasMapper.Object, _friendlyNameGenerator.Object, _logger.Object);

            var postcodes = new List<string>() {"ng1 6dq"};

            var result = await qasAddressGetter.GetPostCodesAndAddressesFromQasAsync(postcodes, cancellationToken);

            _repository.Verify(x => x.SaveAddressesAndFriendlyNameAsync(It.IsAny<IEnumerable<PostcodeDto>>()), Times.Once);

            _qasMapper.Verify(x => x.GetFormatIds(It.IsAny<IEnumerable<QasSearchRootResponse>>()), Times.Once);
            _qasMapper.Verify(x => x.MapToPostcodeDto(It.IsAny<string>(), It.IsAny<IEnumerable<QasFormatRootResponse>>()), Times.Once);

            _qasService.Verify(x => x.GetGlobalIntuitiveSearchResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _qasService.Verify(x => x.GetGlobalIntuitiveFormatResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("NG1 6DQ", result.FirstOrDefault().Postcode);
        }

        [Test]
        public async Task FriendlyNameGeneratorLogsWarningIfItErrors()
        {
            _friendlyNameGenerator.Setup(x => x.GenerateFriendlyName(It.IsAny<PostcodeDto>())).Throws<Exception>();

            CancellationToken cancellationToken = new CancellationToken();
            QasAddressGetter postcodeAndAddressGetter = new QasAddressGetter(_repository.Object, _qasService.Object, _qasMapper.Object, _friendlyNameGenerator.Object, _logger.Object);

            var postcodes = new List<string>() { "ng1 6dq" };

            var result = await postcodeAndAddressGetter.GetPostCodesAndAddressesFromQasAsync(postcodes, cancellationToken);

            _logger.Verify(x => x.LogWarning(It.Is<string>(y => y.Contains("Error generating friendly name")), It.IsAny<Exception>()));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("NG1 6DQ", result.FirstOrDefault().Postcode);
        }
    }
}
