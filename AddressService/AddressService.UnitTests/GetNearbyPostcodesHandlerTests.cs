using AddressService.Core.Config;
using AddressService.Core.Dto;
using AddressService.Core.Services.PostcodeIo;
using AddressService.Handlers;
using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;

namespace AddressService.UnitTests
{
    public class GetNearbyPostcodesHandlerTests
    {

        private Mock<IPostcodeIoService> _postcodeIoService;
        private Mock<IMapper> _mapper;
        private Mock<IPostcodeGetter> _postcodeGetter;
        private Mock<IOptionsSnapshot<ApplicationConfig>> _applicationConfigOptions;

        private ApplicationConfig _applicationConfig;
        [SetUp]
        public void Setup()
        {

            PostCodeIoNearestRootResponse postCodeIoResponse = new PostCodeIoNearestRootResponse()
            {
                Result = new List<PostCodeIoNearestResponse>()
                {
                    new PostCodeIoNearestResponse()
                    {
                        Distance = 2.1, // test response is rounded to nearest metre
                        Postcode = "M11AA"
                    },
                    new PostCodeIoNearestResponse()
                    {
                        Distance = 3,
                        Postcode = "DN551PT"
                    },
                    new PostCodeIoNearestResponse()
                    {
                        Distance = 0.9, // test response is rounded to nearest metre
                        Postcode = "CR26XH"
                    },
                }
            };

            _postcodeIoService = new Mock<IPostcodeIoService>();

            _postcodeIoService.Setup(x => x.GetNearbyPostCodesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(postCodeIoResponse);

            IEnumerable<GetNearbyPostCodeResponse> postcodesReturnedFromMapper = new List<GetNearbyPostCodeResponse>()
            {
                new GetNearbyPostCodeResponse()
                {
                    AddressDetails = new List<AddressDetailsResponse>(),
                    Postcode = "M1 1AA"
                },
                new GetNearbyPostCodeResponse()
                {
                    AddressDetails = new List<AddressDetailsResponse>(),
                    Postcode = "CR2 6XH"
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


            _applicationConfig = new ApplicationConfig()
            {
                NearestPostcodesLimit = 2
            };

            _applicationConfigOptions = new Mock<IOptionsSnapshot<ApplicationConfig>>();

            _applicationConfigOptions.SetupGet(x => x.Value).Returns(_applicationConfig);

        }


        [Test]
        public async Task GetPostcodes_SortedByDistanceAndLimited()
        {
            GetNearbyPostcodesHandler getNearbyPostcodesHandler = new GetNearbyPostcodesHandler(_postcodeIoService.Object, _mapper.Object, _postcodeGetter.Object, _applicationConfigOptions.Object);

            GetNearbyPostcodesRequest request = new GetNearbyPostcodesRequest()
            {
                Postcode = "m11aa"
            };

            GetNearbyPostcodesResponse result = await getNearbyPostcodesHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(2, result.Postcodes.Count);

            // check postcodes have been ordered by ascending distance
            Assert.AreEqual(1, result.Postcodes[0].DistanceInMetres);
            Assert.AreEqual("CR2 6XH", result.Postcodes[0].Postcode);

            Assert.AreEqual(2, result.Postcodes[1].DistanceInMetres);
            Assert.AreEqual("M1 1AA", result.Postcodes[1].Postcode);

            _postcodeIoService.Verify(x => x.GetNearbyPostCodesAsync(It.Is<string>(y => y == "M1 1AA"), It.IsAny<CancellationToken>()));

            // check postcodes have been cleaned, sorted and limited to 2 when getting them from Postcode getter class
            _postcodeGetter.Verify(x => x.GetPostcodesAsync(It.Is<IEnumerable<string>>(y => 
                y.Count() == 2 
                && y.ToList()[0] == "CR2 6XH" 
                && y.ToList()[1] == "M1 1AA"
            ), It.IsAny<CancellationToken>()));

            _mapper.Verify(x => x.Map<IEnumerable<PostcodeDto>, IEnumerable<GetNearbyPostCodeResponse>>(It.IsAny<IEnumerable<PostcodeDto>>()));
        }


    }
}
