using AddressService.Core.Dto;
using AddressService.Core.Services.PostcodeIo;
using AddressService.Core.Utils;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Utils;
using Moq;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.UnitTests
{
    public class PostcodeIoServiceTests
    {

        private Mock<IHttpClientWrapper> _httpClientWrapper;

        [SetUp]
        public void SetUp()
        {
            _httpClientWrapper = new Mock<IHttpClientWrapper>();

            HttpResponseMessage nearestPostcodesResponse = new HttpResponseMessage()
            {
                Content = new StringContent(_nearestPostcodeResponse),
                StatusCode = HttpStatusCode.OK
            };

            _httpClientWrapper.Setup(x => x.GetAsync(It.Is<HttpClientConfigName>(y => y == HttpClientConfigName.PostcodeIo), It.Is<string>(y => y.Contains("nearest")), It.IsAny<CancellationToken>())).ReturnsAsync(nearestPostcodesResponse);


            HttpResponseMessage isPostcodeValidResponse = new HttpResponseMessage()
            {
                Content = new StringContent(_isPostcodeValidResponse),
                StatusCode = HttpStatusCode.OK
            };

            _httpClientWrapper.Setup(x => x.GetAsync(It.Is<HttpClientConfigName>(y => y == HttpClientConfigName.PostcodeIo), It.Is<string>(y => y.Contains("validate")), It.IsAny<CancellationToken>())).ReturnsAsync(isPostcodeValidResponse);
        }

        [Test]
        public async Task GetGlobalIntuitiveFormatResponseAsync()
        {
            PostcodeIoService qasService = new PostcodeIoService(_httpClientWrapper.Object);

            PostCodeIoNearestRootResponse result = await qasService.GetNearbyPostCodesAsync("NG1 5FS", CancellationToken.None);

            Assert.AreEqual("NG1 5FS", result.Result[0].Postcode);
            Assert.AreEqual(1, result.Result[0].Distance);

            _httpClientWrapper.Verify(x => x.GetAsync(It.Is<HttpClientConfigName>(y => y == HttpClientConfigName.PostcodeIo), It.Is<string>(y => y == "postcodes/NG1 5FS/nearest?limit=100&radius=805"), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task GetGlobalIntuitiveSearchResponseAsync()
        {
            PostcodeIoService qasService = new PostcodeIoService(_httpClientWrapper.Object);

            var result = await qasService.IsPostcodeValidAsync("NG1 5FS", CancellationToken.None);

            Assert.AreEqual(true, result);

            _httpClientWrapper.Verify(x => x.GetAsync(It.Is<HttpClientConfigName>(y => y == HttpClientConfigName.PostcodeIo), It.Is<string>(y => y == "postcodes/NG1 5FS/validate"), It.IsAny<CancellationToken>()));
        }



        private string _nearestPostcodeResponse = @"{
	""status"": 200,
	""result"": [
		{
			""postcode"": ""NG1 5FS"",
			""quality"": 1,
			""eastings"": 456848,
			""northings"": 340057,
			""country"": ""England"",
			""nhs_ha"": ""East Midlands"",
			""longitude"": -1.155263,
			""latitude"": 52.954885,
			""european_electoral_region"": ""East Midlands"",
			""primary_care_trust"": ""Nottingham City"",
			""region"": ""East Midlands"",
			""lsoa"": ""Nottingham 022D"",
			""msoa"": ""Nottingham 022"",
			""incode"": ""5FS"",
			""outcode"": ""NG1"",
			""parliamentary_constituency"": ""Nottingham East"",
			""admin_district"": ""Nottingham"",
			""parish"": ""Nottingham, unparished area"",
			""admin_county"": null,
			""admin_ward"": ""Hyson Green & Arboretum"",
			""ced"": null,
			""ccg"": ""NHS Nottingham City"",
			""nuts"": ""Nottingham"",
			""codes"": {
				""admin_district"": ""E06000018"",
				""admin_county"": ""E99999999"",
				""admin_ward"": ""E05012281"",
				""parish"": ""E43000016"",
				""parliamentary_constituency"": ""E14000865"",
				""ccg"": ""E38000132"",
				""ccg_id"": ""04K"",
				""ced"": ""E99999999"",
				""nuts"": ""UKF14""
			},
			""distance"": 1
		}
	]
}";


        private string _isPostcodeValidResponse = @"{""status"":200,""result"":true}";


    }
}
