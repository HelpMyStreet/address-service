using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Dto;
using AddressService.Handlers;
using AddressService.Handlers.BusinessLogic;
using AutoMapper;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using Moq;
using NUnit.Framework;

namespace AddressService.UnitTests.Handlers
{
    public class GetNearbyPostcodesWithoutAddressesHandlerTest
    {
        private Mock<IMapper> _mapper;
        private Mock<INearestPostcodeGetter> _nearestPostcodeGetter;

        private IReadOnlyList<NearestPostcodeDto> _nearestPostcodeDtos;
        private IEnumerable<NearestPostcodeWithoutAddress> _nearestPostcodes;

        [SetUp]
        public void SetUp()
        {
            _mapper = new Mock<IMapper>();

            _nearestPostcodes = new List<NearestPostcodeWithoutAddress>()
            {
                new NearestPostcodeWithoutAddress()
                {
                    Postcode = "NG1 1AA",
                    DistanceInMetres = 99
                }
            };

            _mapper.Setup(x => x.Map<IEnumerable<NearestPostcodeDto>, IEnumerable<NearestPostcodeWithoutAddress>>(It.IsAny<IEnumerable<NearestPostcodeDto>>())).Returns(_nearestPostcodes);


            _nearestPostcodeGetter = new Mock<INearestPostcodeGetter>();

            _nearestPostcodeDtos = new List<NearestPostcodeDto>()
            {
                new NearestPostcodeDto()
                {
                    Postcode = "NG1 1AA",
                    DistanceInMetres = 99
                }
            };
            _nearestPostcodeGetter.Setup(x => x.GetNearestPostcodesAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>())).ReturnsAsync(_nearestPostcodeDtos);
        }

        [Test]
        public async Task GetNumberOfAddressesPerPostcodeInBoundaryResponse()
        {
            GetNearbyPostcodesWithoutAddressesRequest request = new GetNearbyPostcodesWithoutAddressesRequest()
            {
                Postcode = "NG 2AA",
                MaxNumberOfResults = 1,
                RadiusInMetres = 200
            };

            GetNearbyPostcodesWithoutAddressesHandler getNearbyPostcodesWithoutAddressesHandler = new GetNearbyPostcodesWithoutAddressesHandler(_mapper.Object, _nearestPostcodeGetter.Object);

            GetNearbyPostcodesWithoutAddressesResponse result = await getNearbyPostcodesWithoutAddressesHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(1, result.NearestPostcodes.Count);
            Assert.AreEqual("NG1 1AA", result.NearestPostcodes.FirstOrDefault().Postcode);
            Assert.AreEqual(99, result.NearestPostcodes.FirstOrDefault().DistanceInMetres);

            _nearestPostcodeGetter.Setup(x => x.GetNearestPostcodesAsync(It.Is<string>(y => y == "NG 2AA"), It.Is<int?>(y => y == 200), It.Is<int?>(y => y == 1)));

            _mapper.Verify(x => x.Map<IEnumerable<NearestPostcodeDto>, IEnumerable<NearestPostcodeWithoutAddress>>(It.Is<IEnumerable<NearestPostcodeDto>>(y => y.Count() == 1 && y.Any(z => z.Postcode == "NG1 1AA"))), Times.Once);
        }
    }
}
