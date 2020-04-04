using AddressService.Core.Dto;
using AddressService.Handlers;
using AutoMapper;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
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
            GetPostcodeResponse getNearbyGetPostcodesResponse = new GetPostcodeResponse()
            {
                Postcode = "NG1 5FS"
            };

            _mapper = new Mock<IMapper>();
            _mapper.Setup(x => x.Map<PostcodeDto, GetPostcodeResponse>(It.IsAny<PostcodeDto>())).Returns(getNearbyGetPostcodesResponse);

            _postcodeGetter = new Mock<IPostcodeGetter>();
            _postcodeGetter.SetupAllProperties();
        }

        [Test]
        public async Task GetPostcode()
        {
            GetPostcodeHandler getPostcodeHandler = new GetPostcodeHandler(_mapper.Object, _postcodeGetter.Object);

            GetPostcodeRequest request = new GetPostcodeRequest()
            {
                Postcode = "ng15fs"
            };

            GetPostcodeResponse result = await getPostcodeHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual("NG1 5FS", result.Postcode);
            _postcodeGetter.Verify(x => x.GetPostcodeAsync(It.Is<string>(y => y == "NG1 5FS"), It.IsAny<CancellationToken>()));
            _mapper.Verify(x => x.Map<PostcodeDto, GetPostcodeResponse>(It.IsAny<PostcodeDto>()));
        }
    }
}
