using AddressService.Core.Dto;
using AddressService.Handlers;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Handlers.BusinessLogic;

namespace AddressService.UnitTests
{
    public class GetPostcodeCoordinatesHandlerTests
    {
        private Mock<IPostcodeCoordinatesGetter> _postcodeCoordinatesGetter;

        private IReadOnlyDictionary<string, CoordinatesDto> _postcodeCoordinates;
        [SetUp]
        public void SetUp()
        {

            _postcodeCoordinates = new Dictionary<string, CoordinatesDto>()
            {
                { "NG1 5FS", new CoordinatesDto("NG1 5FS", 52.954885, -1.155263)},
                { "NG1 5BL", new CoordinatesDto("NG1 5BL",52.955494, -1.154864)}, 

            };

            _postcodeCoordinatesGetter = new Mock<IPostcodeCoordinatesGetter>();
            _postcodeCoordinatesGetter.Setup(x => x.GetPostcodeCoordinatesAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(_postcodeCoordinates);
        }
        
        [Test]
        public async Task GetPostcodeCoordinates()
        {

            GetPostcodeCoordinatesRequest getPostcodeCoordinatesRequest = new GetPostcodeCoordinatesRequest()
            {
                Postcodes = new List<string>()
                {
                    "NG1 5FS", "NG1 5BL"
                }
            };

            GetPostcodeCoordinatesHandler getPostcodeCoordinatesHandler = new GetPostcodeCoordinatesHandler(_postcodeCoordinatesGetter.Object);

            GetPostcodeCoordinatesResponse result = await getPostcodeCoordinatesHandler.Handle(getPostcodeCoordinatesRequest, CancellationToken.None);

            Assert.AreEqual(2,result.PostcodeCoordinates.Count());

            Assert.AreEqual(52.954885,result.PostcodeCoordinates.FirstOrDefault(x=>x.Postcode == "NG1 5FS").Latitude);
            Assert.AreEqual(-1.155263, result.PostcodeCoordinates.FirstOrDefault(x=>x.Postcode == "NG1 5FS").Longitude);

            Assert.AreEqual(52.955494, result.PostcodeCoordinates.FirstOrDefault(x => x.Postcode == "NG1 5BL").Latitude);
            Assert.AreEqual(-1.154864, result.PostcodeCoordinates.FirstOrDefault(x => x.Postcode == "NG1 5BL").Longitude);


        }
    }
}
