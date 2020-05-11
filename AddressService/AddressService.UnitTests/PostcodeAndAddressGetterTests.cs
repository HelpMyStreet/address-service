using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Handlers.BusinessLogic;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.UnitTests
{
    public class PostcodeAndAddressGetterTests
    {
        private Mock<IRepository> _repository;
        private Mock<IQasAddressGetter> _qasAddressGetter;
        private Mock<IPostcodesWithoutAddressesCache> _postcodesWithoutAddressesCache;

        private IEnumerable<PostcodeDto> _postcodeDtosInDbs;
        private IEnumerable<PostcodeDto> _postcodeDtosFromQas;

        private IEnumerable<PostcodeWithNumberOfAddressesDto> _postCodesWithNumberOfAddresses;

        private HashSet<string> _postCodesWithoutAddresses;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IRepository>();
            _repository.SetupAllProperties();

            _postcodeDtosInDbs = new List<PostcodeDto>()
            {
                new PostcodeDto()
                {
                    Id = 1,
                    Postcode = "NG1 1AA",
                    AddressDetails = new List<AddressDetailsDto>()
                    {
                        new AddressDetailsDto()
                        {
                            AddressLine1 = "1_addressline1",
                            AddressLine2 = "1_addressline2",
                            AddressLine3 = "1_addressline1",
                            Locality = "1_locality"
                        }
                    }
                }
            };


            _repository.Setup(x => x.GetPostcodesAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(_postcodeDtosInDbs);

            _postCodesWithNumberOfAddresses = new List<PostcodeWithNumberOfAddressesDto>()
            {
                new PostcodeWithNumberOfAddressesDto()
                {
                    Postcode = "NG1 1AA",
                    NumberOfAddresses = 3
                }
            };

            _repository.Setup(x => x.GetNumberOfAddressesPerPostcodeAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(_postCodesWithNumberOfAddresses);



            _qasAddressGetter = new Mock<IQasAddressGetter>();

            _postcodeDtosFromQas = new List<PostcodeDto>()
            {
                new PostcodeDto()
                {
                    Id = 2,
                    Postcode = "NG1 1AB",
                    AddressDetails = new List<AddressDetailsDto>()
                    {
                        new AddressDetailsDto()
                        {
                            AddressLine1 = "2_addressline1",
                            AddressLine2 = "2_addressline2",
                            AddressLine3 = "2_addressline1",
                            Locality = "2_locality"
                        },
                        new AddressDetailsDto()
                        {
                            AddressLine1 = "3_addressline1",
                            AddressLine2 = "3_addressline2",
                            AddressLine3 = "3_addressline1",
                            Locality = "3_locality"
                        }
                    }
                }
            };

            _qasAddressGetter.Setup(x => x.GetPostCodesAndAddressesFromQasAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(_postcodeDtosFromQas);

            _postcodesWithoutAddressesCache = new Mock<IPostcodesWithoutAddressesCache>();

            _postcodesWithoutAddressesCache.SetupAllProperties();
            _postCodesWithoutAddresses = new HashSet<string>() { "NG1 1AC" };

            _postcodesWithoutAddressesCache.SetupGet(x => x.PostcodesWithoutAddresses).Returns(_postCodesWithoutAddresses);
        }

        [Test]
        public async Task GetPostcodesAsync_PostCodesAreRetrievedFromQasAndDatabase()
        {
            CancellationToken cancellationToken = new CancellationToken();
            PostcodeAndAddressGetter postcodeAndAddressGetter = new PostcodeAndAddressGetter(_repository.Object, _qasAddressGetter.Object, _postcodesWithoutAddressesCache.Object);

            List<string> postcodes = new List<string>()
            {
                "NG11AA", // in DB
                "NG11AB", // addresses aren't in DB, but will be retrieved through QAS
                "NG1 1AC", // will be filtered out when calling QAS because of postcodes without addresses cache
                "NG1 1AD", // will be added to postcodes without addresses cache
            };

            IEnumerable<PostcodeDto> result = await postcodeAndAddressGetter.GetPostcodesAsync(postcodes, cancellationToken);

            _repository.Verify(x => x.GetPostcodesAsync(It.Is<IEnumerable<string>>(y => y.Count() == 4 && y.Contains("NG1 1AA") && y.Contains("NG1 1AB") && y.Contains("NG1 1AC") && y.Contains("NG1 1AD"))), Times.Once);


            _qasAddressGetter.Verify(x => x.GetPostCodesAndAddressesFromQasAsync(It.Is<IEnumerable<string>>(y => y.Count() == 2 && y.Contains("NG1 1AB") && y.Contains("NG1 1AD")), It.IsAny<CancellationToken>()), Times.Once); // doesn't include NG1 1AC because it's been filtered out before calling QAS

            _postcodesWithoutAddressesCache.Verify(x => x.AddRange(It.Is<IEnumerable<string>>(y => y.Count() == 1 && y.Contains("NG1 1AD"))));

            Assert.AreEqual(2, result.Count());

            List<AddressDetailsDto> returnedMissingPostCodeAddressFromDb = result.FirstOrDefault(x => x.Postcode == "NG1 1AA").AddressDetails;
            List<AddressDetailsDto> returnedMissingPostCodeAddressFromQas = result.FirstOrDefault(x => x.Postcode == "NG1 1AB").AddressDetails;

            Assert.AreEqual(1, returnedMissingPostCodeAddressFromDb.Count());
            Assert.AreEqual("1_addressline1", returnedMissingPostCodeAddressFromDb.FirstOrDefault().AddressLine1);
            Assert.AreEqual("1_addressline2", returnedMissingPostCodeAddressFromDb.FirstOrDefault().AddressLine2);
            Assert.AreEqual("1_addressline1", returnedMissingPostCodeAddressFromDb.FirstOrDefault().AddressLine3);
            Assert.AreEqual("1_locality", returnedMissingPostCodeAddressFromDb.FirstOrDefault().Locality);

            Assert.AreEqual(2, returnedMissingPostCodeAddressFromQas.Count());
            Assert.AreEqual("2_addressline1", returnedMissingPostCodeAddressFromQas.FirstOrDefault(x=>x.AddressLine1 == "2_addressline1").AddressLine1);
            Assert.AreEqual("2_addressline2", returnedMissingPostCodeAddressFromQas.FirstOrDefault(x => x.AddressLine1 == "2_addressline1").AddressLine2);
            Assert.AreEqual("2_addressline1", returnedMissingPostCodeAddressFromQas.FirstOrDefault(x => x.AddressLine1 == "2_addressline1").AddressLine3);
            Assert.AreEqual("2_locality", returnedMissingPostCodeAddressFromQas.FirstOrDefault(x => x.AddressLine1 == "2_addressline1").Locality);

        }

        [Test]
        public async Task GetPostcodeAsync()
        {
            CancellationToken cancellationToken = new CancellationToken();
            PostcodeAndAddressGetter postcodeAndAddressGetter = new PostcodeAndAddressGetter(_repository.Object, _qasAddressGetter.Object, _postcodesWithoutAddressesCache.Object);

            PostcodeDto result = await postcodeAndAddressGetter.GetPostcodeAsync("NG11AA", cancellationToken);

            _repository.Verify(x => x.GetPostcodesAsync(It.Is<IEnumerable<string>>(y => y.Count() == 1 && y.Contains("NG1 1AA"))), Times.Once);

            Assert.AreEqual(1, result.AddressDetails.Count());
            Assert.AreEqual("1_addressline1", result.AddressDetails.FirstOrDefault().AddressLine1);
            Assert.AreEqual("1_addressline2", result.AddressDetails.FirstOrDefault().AddressLine2);
            Assert.AreEqual("1_addressline1", result.AddressDetails.FirstOrDefault().AddressLine3);
            Assert.AreEqual("1_locality", result.AddressDetails.FirstOrDefault().Locality);
        }


        [Test]
        public async Task GetNumberOfAddressesPerPostcodeAsync()
        {
            CancellationToken cancellationToken = new CancellationToken();
            PostcodeAndAddressGetter postcodeAndAddressGetter = new PostcodeAndAddressGetter(_repository.Object, _qasAddressGetter.Object, _postcodesWithoutAddressesCache.Object);

            List<string> postcodes = new List<string>()
            {
                "NG11AA", // in DB
                "NG11AB", // addresses aren't in DB, but will be retrieved through QAS
                "NG1 1AC", // will be filtered out because of postcodes without addresses cache
                "NG1 1AD", // will be added to postcodes without addresses cache
            };

            IEnumerable<PostcodeWithNumberOfAddressesDto> result = await postcodeAndAddressGetter.GetNumberOfAddressesPerPostcodeAsync(postcodes, cancellationToken);

            _repository.Verify(x => x.GetNumberOfAddressesPerPostcodeAsync(It.Is<IEnumerable<string>>(y => y.Count() == 3 && y.Contains("NG1 1AA") && y.Contains("NG1 1AB") && y.Contains("NG1 1AD"))), Times.Once);


            _qasAddressGetter.Verify(x => x.GetPostCodesAndAddressesFromQasAsync(It.Is<IEnumerable<string>>(y => y.Count() == 2 && y.Contains("NG1 1AB") && y.Contains("NG1 1AD")), It.IsAny<CancellationToken>()), Times.Once); // doesn't include NG1 1AC because it's been filtered out


            _postcodesWithoutAddressesCache.Verify(x => x.AddRange(It.Is<IEnumerable<string>>(y => y.Count() == 1 && y.Contains("NG1 1AD"))));

            Assert.AreEqual(2, result.Count());

            Assert.AreEqual(3, result.FirstOrDefault(x => x.Postcode == "NG1 1AA").NumberOfAddresses);
            Assert.AreEqual(2, result.FirstOrDefault(x => x.Postcode == "NG1 1AB").NumberOfAddresses);
        }

    }
}
