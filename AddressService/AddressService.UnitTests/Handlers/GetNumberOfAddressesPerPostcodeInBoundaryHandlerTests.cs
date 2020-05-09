using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Handlers;
using AddressService.Handlers.BusinessLogic;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;

namespace AddressService.UnitTests.Handlers
{
    public class GetNumberOfAddressesPerPostcodeInBoundaryHandlerTests
    {
        private Mock<IRepository> _repository;
        private Mock<IPostcodeAndAddressGetter> _postcodeAndAddressGetter;

        private IEnumerable<string> _postcodes;
        private IEnumerable<PostcodeWithNumberOfAddressesDto> _postCodesWithNumberOfAddresses;

        [SetUp]
        public void SetUp()
        {

            _repository = new Mock<IRepository>();

            _postcodes = new List<string>()
            {
                "NG1 1AA"
            };
            _repository.Setup(x => x.GetPostcodesInBoundaryAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())).ReturnsAsync(_postcodes);


            //,It.IsAny<CancellationToken>()
            _postcodeAndAddressGetter = new Mock<IPostcodeAndAddressGetter>();

            _postCodesWithNumberOfAddresses = new List<PostcodeWithNumberOfAddressesDto>()
            {
                new PostcodeWithNumberOfAddressesDto()
                {
                    Postcode = "NG1 1AA",
                    NumberOfAddresses = 3
                }
            };

            _postcodeAndAddressGetter.Setup(x => x.GetNumberOfAddressesPerPostcodeAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(_postCodesWithNumberOfAddresses);

        }

        [Test]
        public async Task GetNumberOfAddressesPerPostcodeInBoundaryResponse()
        {
            GetNumberOfAddressesPerPostcodeInBoundaryRequest request = new GetNumberOfAddressesPerPostcodeInBoundaryRequest()
            {
                SwLatitude = 1,
                SwLongitude = 2,
                NeLatitude = 3,
                NeLongitude = 4
            };

            GetNumberOfAddressesPerPostcodeInBoundaryHandler getNumberOfAddressesPerPostcodeInBoundaryHandler = new GetNumberOfAddressesPerPostcodeInBoundaryHandler(_repository.Object, _postcodeAndAddressGetter.Object);

            GetNumberOfAddressesPerPostcodeInBoundaryResponse result = await getNumberOfAddressesPerPostcodeInBoundaryHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(1, result.PostcodesWithNumberOfAddresses.Count);
            Assert.AreEqual(3, result.PostcodesWithNumberOfAddresses.FirstOrDefault().NumberOfAddresses);
            Assert.AreEqual("NG1 1AA", result.PostcodesWithNumberOfAddresses.FirstOrDefault().Postcode);

            _repository.Verify(x => x.GetPostcodesInBoundaryAsync(It.Is<double>(y => y == 1d), It.Is<double>(y => y == 2d), It.Is<double>(y => y == 3d), It.Is<double>(y => y == 4d)), Times.Once);

            _postcodeAndAddressGetter.Verify(x => x.GetNumberOfAddressesPerPostcodeAsync(It.Is<IEnumerable<string>>(y => y.Count() == 1 && y.Contains("NG1 1AA")), It.IsAny<CancellationToken>()));
        }
    }
}
