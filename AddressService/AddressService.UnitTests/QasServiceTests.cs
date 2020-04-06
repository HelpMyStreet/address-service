using AddressService.Core.Config;
using AddressService.Core.Dto;
using AddressService.Core.Services.Qas;
using AddressService.Core.Utils;
using Moq;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.UnitTests
{
    public class QasServiceTests
    {

        private Mock<IHttpClientWrapper> _httpClientWrapper;


        [SetUp]
        public void SetUp()
        {
            _httpClientWrapper = new Mock<IHttpClientWrapper>();

            HttpResponseMessage searchResponse = new HttpResponseMessage()
            {
                Content = new StringContent(_searchResponse),
                StatusCode = HttpStatusCode.OK
            };
            
            _httpClientWrapper.Setup(x => x.GetAsync(It.Is<HttpClientConfigName>(y => y == HttpClientConfigName.Qas), It.Is<string>(y => y.Contains("capture/address/v2/search")), It.IsAny<CancellationToken>())).ReturnsAsync(searchResponse);


            HttpResponseMessage formatResponse = new HttpResponseMessage()
            {
                Content = new StringContent(_formatResponse),
                StatusCode = HttpStatusCode.OK
            };

            _httpClientWrapper.Setup(x => x.GetAsync(It.Is<HttpClientConfigName>(y => y == HttpClientConfigName.Qas), It.Is<string>(y => y.Contains("capture/address/v2/format")), It.IsAny<CancellationToken>())).ReturnsAsync(formatResponse);
        }

        [Test]
        public async Task GetGlobalIntuitiveFormatResponseAsync()
        {
            QasService qasService = new QasService(_httpClientWrapper.Object);

            QasFormatRootResponse result = await qasService.GetGlobalIntuitiveFormatResponseAsync("aWQ9NTUwMTgxNTZ-Zm9ybWF0aWQ9NTA3Y2Y5YmItNzA0MC00NGVhLWJiZmItNzE0ZDhmNWIxOWJhfnFsPTZ-Z2VvPTA", CancellationToken.None);

            Assert.AreEqual("Experian Data Quality", result.Address[0].AddressLine1);

            _httpClientWrapper.Verify(x => x.GetAsync(It.Is<HttpClientConfigName>(y => y == HttpClientConfigName.Qas), It.Is<string>(y => y == "capture/address/v2/format?country=GBR&id=aWQ9NTUwMTgxNTZ-Zm9ybWF0aWQ9NTA3Y2Y5YmItNzA0MC00NGVhLWJiZmItNzE0ZDhmNWIxOWJhfnFsPTZ-Z2VvPTA"), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task GetGlobalIntuitiveSearchResponseAsync()
        {
            QasService qasService = new QasService(_httpClientWrapper.Object);

            QasSearchRootResponse result = await qasService.GetGlobalIntuitiveSearchResponseAsync("NG15FS", CancellationToken.None);

            Assert.AreEqual("NG1 5FS", result.Postcode);
            Assert.AreEqual("string", result.Results[0].Suggestion);

            _httpClientWrapper.Verify(x => x.GetAsync(It.Is<HttpClientConfigName>(y => y == HttpClientConfigName.Qas), It.Is<string>(y => y == "capture/address/v2/search?query=NG15FS&country=GBR&take=100"), It.IsAny<CancellationToken>()));
        }



        private string _searchResponse = @"{
  ""totalMatches"": 49932,
  ""count"": 7,
  ""results"": [
    {
      ""suggestion"": ""string"",
      ""matched"": [
        [
          13
        ]
      ],
      ""format"": ""string""
    }
  ]
}";


        private string _formatResponse = @"{
  ""address"": [
    {
      ""addressLine1"": ""Experian Data Quality""
    },
    {
      ""addressLine2"": ""George West House""
    },
    {
      ""addressLine3"": ""2-3 Clapham Common North Side""
    },
    {
      ""locality"": ""LONDON""
    },
    {
      ""province"": """"
    },
    {
      ""postalCode"": ""SW4 0QL""
    },
    {
      ""country"": ""UNITED KINGDOM""
    }
  ],
  ""components"": [
    {
      ""building1"": ""George West House""
    },
    {
      ""organisation1"": ""Experian Data Quality""
    },
    {
      ""streetNumber1"": ""2-3""
    },
    {
      ""street1"": ""Clapham Common North Side""
    },
    {
      ""locality1"": ""LONDON""
    },
    {
      ""postalCode1"": ""SW4 0QL""
    },
    {
      ""country1"": ""UNITED KINGDOM""
    },
    {
      ""countryISO1"": ""GBR""
    }
  ],
  ""metadata"": {
    ""dpv"": {
      ""cmraIndicator"": "" "",
      ""seedIndicator"": ""N"",
      ""dpvIndicator"": ""N"",
      ""footnotes"": [
        ""AA"",
        ""M3"",
        "" ""
      ],
      ""vacancyIndicator"": "" "",
      ""noStatsIndicator"": "" "",
      ""pbsaIndicator"": "" ""
    }
  }
}";


    }
}
