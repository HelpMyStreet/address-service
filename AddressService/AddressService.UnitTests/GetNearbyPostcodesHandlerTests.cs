using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Config;
using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Dto;
using AddressService.Core.Services.PostcodeIo;
using AddressService.Handlers;
using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

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
                        Distance = 2,
                        Postcode = "M11AA"
                    },
                    new PostCodeIoNearestResponse()
                    {
                        Distance = 3,
                        Postcode = "DN551PT"
                    },
                    new PostCodeIoNearestResponse()
                    {
                        Distance = 1,
                        Postcode = "CR26XH"
                    },
                }
            };

            _postcodeIoService = new Mock<IPostcodeIoService>();

            _postcodeIoService.Setup(x => x.GetNearbyPostCodesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(postCodeIoResponse);

            IEnumerable<PostcodeResponse> postcodesReturnedFromMapper = new List<PostcodeResponse>()
            {
                new PostcodeResponse()
                {
                    AddressDetails = new List<AddressDetailsResponse>(),
                    PostCode = "M1 1AA"
                }
            };


            _mapper = new Mock<IMapper>();

            _mapper.Setup(x => x.Map<IEnumerable<PostcodeDto>, IEnumerable<PostcodeResponse>>(It.IsAny<IEnumerable<PostcodeDto>>())).Returns(postcodesReturnedFromMapper);

            IEnumerable<PostcodeDto> postcodeDtos = new List<PostcodeDto>();

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

            Assert.AreEqual("M1 1AA", result.Postcodes.FirstOrDefault().PostCode);


            _postcodeIoService.Verify(x => x.GetNearbyPostCodesAsync(It.Is<string>(y => y == "M1 1AA"), It.IsAny<CancellationToken>()));

            // check postcodes have been cleaned, sorted and limited to 2
            _postcodeGetter.Verify(x => x.GetPostcodesAsync(It.Is<IEnumerable<string>>(y => y.Count() == 2 && y.ToList()[0] == "CR2 6XH" && y.ToList()[1] == "M1 1AA"
            ), It.IsAny<CancellationToken>()));

            _mapper.Verify(x => x.Map<IEnumerable<PostcodeDto>, IEnumerable<PostcodeResponse>>(It.IsAny<IEnumerable<PostcodeDto>>()));
        }


    }
}
