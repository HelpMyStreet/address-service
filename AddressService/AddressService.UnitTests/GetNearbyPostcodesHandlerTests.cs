using AddressService.Core.Dto;
using AddressService.Core.Utils;
using AddressService.Handlers;
using AutoMapper;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.UnitTests
{
    public class GetNearbyPostcodesHandlerTests
    {
        private Mock<INearestPostcodeGetter> _nearestPostcodeGetter;
        private Mock<IMapper> _mapper;
        private Mock<IPostcodeGetter> _postcodeGetter;
        private Mock<IAddressDetailsSorter> _addressDetailsSorter;

        [SetUp]
        public void Setup()
        {
            IReadOnlyList<NearestPostcodeDto> nearestPostcodeDtos = new List<NearestPostcodeDto>()
            {
                new NearestPostcodeDto()
                {
                DistanceInMetres = 2,
                Postcode = "M1 1AA"
            },
            new NearestPostcodeDto()
            {
                DistanceInMetres = 3,
                Postcode = "DN55 1PT"
            },
            new NearestPostcodeDto()
            {
                DistanceInMetres = 1,
                Postcode = "CR2 6XH"
            },
            };

            _nearestPostcodeGetter = new Mock<INearestPostcodeGetter>();

            _nearestPostcodeGetter.Setup(x => x.GetNearestPostcodesAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>())).ReturnsAsync(nearestPostcodeDtos);

            IEnumerable<GetNearbyPostCodeResponse> postcodesReturnedFromMapper = new List<GetNearbyPostCodeResponse>()
            {
                new GetNearbyPostCodeResponse()
                {
                    AddressDetails = new List<AddressDetailsResponse>(),
                    Postcode = "M1 1AA",
                    FriendlyName = "friendlyName1"
                },
                new GetNearbyPostCodeResponse()
                {
                    AddressDetails = new List<AddressDetailsResponse>(),
                    Postcode = "CR2 6XH",
                    FriendlyName = "friendlyName2"
                },
            };


            _mapper = new Mock<IMapper>();

            _mapper.Setup(x => x.Map<IEnumerable<PostcodeDto>, IEnumerable<GetNearbyPostCodeResponse>>(It.IsAny<IEnumerable<PostcodeDto>>())).Returns(postcodesReturnedFromMapper);

            IEnumerable<PostcodeDto> postcodeDtos = new List<PostcodeDto>()
            {
                new PostcodeDto()
                {
                    Postcode = "M1 1AA",
                    AddressDetails = new List<AddressDetailsDto>()
                },
                new PostcodeDto()
                {
                    Postcode = "CR2 6XH",
                    AddressDetails = new List<AddressDetailsDto>()
                },
            };

            _postcodeGetter = new Mock<IPostcodeGetter>();

            _postcodeGetter.Setup(x => x.GetPostcodesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(postcodeDtos);

            _addressDetailsSorter = new Mock<IAddressDetailsSorter>();
            _addressDetailsSorter.SetupAllProperties();
        }


        [Test]
        public async Task GetPostcodes_SortedByDistanceAndLimited()
        {
            GetNearbyPostcodesHandler getNearbyPostcodesHandler = new GetNearbyPostcodesHandler(_nearestPostcodeGetter.Object, _mapper.Object, _postcodeGetter.Object, _addressDetailsSorter.Object);

            GetNearbyPostcodesRequest request = new GetNearbyPostcodesRequest()
            {
                Postcode = "m11aa"
            };

            GetNearbyPostcodesResponse result = await getNearbyPostcodesHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(2, result.Postcodes.Count);

            // check postcodes have been ordered by ascending distance
            Assert.AreEqual(1, result.Postcodes[0].DistanceInMetres);
            Assert.AreEqual("CR2 6XH", result.Postcodes[0].Postcode);
            Assert.AreEqual("friendlyName2", result.Postcodes[0].FriendlyName);

            Assert.AreEqual(2, result.Postcodes[1].DistanceInMetres);
            Assert.AreEqual("M1 1AA", result.Postcodes[1].Postcode);
            Assert.AreEqual("friendlyName1", result.Postcodes[1].FriendlyName);

            _nearestPostcodeGetter.Verify(x => x.GetNearestPostcodesAsync(It.Is<string>(y => y == "M1 1AA"), It.Is<int?>(y => y == null), It.Is<int?>(y => y == null)));

            _postcodeGetter.Verify(x => x.GetPostcodesAsync(It.Is<IEnumerable<string>>(y =>
                         y.Count() == 3
                     ), It.IsAny<CancellationToken>()));

            _mapper.Verify(x => x.Map<IEnumerable<PostcodeDto>, IEnumerable<GetNearbyPostCodeResponse>>(It.IsAny<IEnumerable<PostcodeDto>>()));

            _addressDetailsSorter.Verify(x => x.OrderAddressDetailsResponse(It.IsAny<IEnumerable<AddressDetailsResponse>>()));
        }


    }
}
