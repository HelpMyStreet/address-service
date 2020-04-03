using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Dto;
using AddressService.Handlers;
using AutoMapper;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.UnitTests
{
    public class GetPostcodeHandlerTests
    {
        private Mock<IMapper> _mapper;
        private Mock<IPostcodeGetter> _postcodeGetter;


        [SetUp]
        public void SetUp()
        {
            PostcodeResponse getNearbyPostcodesResponse = new PostcodeResponse()
            {
                PostCode = "NG1 5FS"
            };

            _mapper = new Mock<IMapper>();
            _mapper.Setup(x => x.Map<PostcodeDto, PostcodeResponse>(It.IsAny<PostcodeDto>())).Returns(getNearbyPostcodesResponse);

            _postcodeGetter = new Mock<IPostcodeGetter>();
            _postcodeGetter.SetupAllProperties();
        }

        [Test]
        public async Task GetPostcode()
        {
            GetPostcodeHandler getPostcodeHandler = new GetPostcodeHandler(_mapper.Object, _postcodeGetter.Object);

            GetPostcodeRequest request = new GetPostcodeRequest()
            {
                Postcode = "NG1 5FS"
            };

            PostcodeResponse result = await getPostcodeHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual("NG1 5FS", result.PostCode);
            _postcodeGetter.Verify(x => x.GetPostcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));
            _mapper.Verify(x => x.Map<PostcodeDto, PostcodeResponse>(It.IsAny<PostcodeDto>()));
        }
    }
}
