using AddressService.Core.Config;
using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Handlers;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AddressService.Handlers.BusinessLogic;

namespace AddressService.UnitTests
{
    public class NearestPostcodeGetterTests
    {
        private Mock<IRepository> _repository;
        private Mock<IOptionsSnapshot<ApplicationConfig>> _applicationConfig;

        private IEnumerable<NearestPostcodeDto> _nearestPostcodeDtos;
        private ApplicationConfig _applicationConfigSettings;

        private string _postcode = "NG1 5FS";
        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IRepository>();

            _repository.SetupAllProperties();

            _nearestPostcodeDtos = new List<NearestPostcodeDto>()
            {
                new NearestPostcodeDto()
                {
                    Postcode = "NG1 AA",
                    DistanceInMetres = 2
                },
                new NearestPostcodeDto()
                {
                    Postcode = "NG1 AB",
                    DistanceInMetres = 3
                },
                new NearestPostcodeDto()
                {
                    Postcode = "NG1 AC",
                    DistanceInMetres = 1
                },
                new NearestPostcodeDto()
                {
                    Postcode = "NG1 AD",
                    DistanceInMetres = 4
                },
                new NearestPostcodeDto()
                {
                Postcode = "NG1 AE",
                DistanceInMetres = 16095
            },
            };

            _repository.Setup(x => x.GetNearestPostcodesAsync(It.Is<string>(y => y == _postcode), It.IsAny<double>())).ReturnsAsync(_nearestPostcodeDtos);

            PreComputedNearestPostcodesDto preComputedNearestPostcodes = NearestPostcodeCompressor.CompressNearestPostcodeDtos(_postcode, _nearestPostcodeDtos);

            _repository.Setup(x => x.GetPreComputedNearestPostcodes(It.Is<string>(y => y == _postcode))).ReturnsAsync(preComputedNearestPostcodes);

            _applicationConfigSettings = new ApplicationConfig()
            {
                DefaultMaxNumberOfNearbyPostcodes = 0,
                DefaultNearestPostcodeRadiusInMetres = 0
            };

            _applicationConfig = new Mock<IOptionsSnapshot<ApplicationConfig>>();
            _applicationConfig.SetupGet(x => x.Value).Returns(_applicationConfigSettings);
        }

        [Test]
        public async Task GetPostcodesFromCache_ResultsAreOrdered_LimitedByRadius()
        {
            NearestPostcodeGetter nearestPostcodeGetter = new NearestPostcodeGetter(_repository.Object, _applicationConfig.Object);

            IReadOnlyList<NearestPostcodeDto> result = await nearestPostcodeGetter.GetNearestPostcodesAsync(_postcode, 3, 5);

            _repository.Verify(x => x.GetNearestPostcodesAsync(It.IsAny<string>(), It.IsAny<double>()), Times.Never);

            List<NearestPostcodeDto> expectedResult = _nearestPostcodeDtos.Where(x => x.DistanceInMetres <= 3).OrderBy(x => x.DistanceInMetres).Take(5).ToList();


            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(expectedResult[i].Postcode, result[i].Postcode);
                Assert.AreEqual(expectedResult[i].DistanceInMetres, result[i].DistanceInMetres);
            }
        }

        [Test]
        public async Task GetPostcodesFromCache_ResultsAreOrdered_LimitedByDefaultRadius()
        {
            _applicationConfigSettings = new ApplicationConfig()
            {
                DefaultMaxNumberOfNearbyPostcodes = 0,
                DefaultNearestPostcodeRadiusInMetres = 3
            };
            _applicationConfig.SetupGet(x => x.Value).Returns(_applicationConfigSettings);

            NearestPostcodeGetter nearestPostcodeGetter = new NearestPostcodeGetter(_repository.Object, _applicationConfig.Object);

            IReadOnlyList<NearestPostcodeDto> result = await nearestPostcodeGetter.GetNearestPostcodesAsync(_postcode, null, 5);

            _repository.Verify(x => x.GetNearestPostcodesAsync(It.IsAny<string>(), It.IsAny<double>()), Times.Never);

            List<NearestPostcodeDto> expectedResult = _nearestPostcodeDtos.Where(x => x.DistanceInMetres <= 3).OrderBy(x => x.DistanceInMetres).Take(5).ToList();


            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(expectedResult[i].Postcode, result[i].Postcode);
                Assert.AreEqual(expectedResult[i].DistanceInMetres, result[i].DistanceInMetres);
            }
        }

        [Test]
        public async Task GetPostcodesFromCache_ResultsAreOrdered_LimitedByMaxNumberOfResults()
        {
            NearestPostcodeGetter nearestPostcodeGetter = new NearestPostcodeGetter(_repository.Object, _applicationConfig.Object);

            IReadOnlyList<NearestPostcodeDto> result = await nearestPostcodeGetter.GetNearestPostcodesAsync(_postcode, 5, 3);

            _repository.Verify(x => x.GetNearestPostcodesAsync(It.IsAny<string>(), It.IsAny<double>()), Times.Never);

            List<NearestPostcodeDto> expectedResult = _nearestPostcodeDtos.Where(x => x.DistanceInMetres <= 5).OrderBy(x => x.DistanceInMetres).Take(3).ToList();


            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(expectedResult[i].Postcode, result[i].Postcode);
                Assert.AreEqual(expectedResult[i].DistanceInMetres, result[i].DistanceInMetres);
            }
        }

        [Test]
        public async Task GetPostcodesFromCache_ResultsAreOrdered_LimitedByDefaultMaxNumberOfResults()
        {
            _applicationConfigSettings = new ApplicationConfig()
            {
                DefaultMaxNumberOfNearbyPostcodes = 3,
                DefaultNearestPostcodeRadiusInMetres = 0
            };
            _applicationConfig.SetupGet(x => x.Value).Returns(_applicationConfigSettings);

            NearestPostcodeGetter nearestPostcodeGetter = new NearestPostcodeGetter(_repository.Object, _applicationConfig.Object);

            IReadOnlyList<NearestPostcodeDto> result = await nearestPostcodeGetter.GetNearestPostcodesAsync(_postcode, 5, null);

            _repository.Verify(x => x.GetNearestPostcodesAsync(It.IsAny<string>(), It.IsAny<double>()), Times.Never);

            List<NearestPostcodeDto> expectedResult = _nearestPostcodeDtos.Where(x => x.DistanceInMetres <= 5).OrderBy(x => x.DistanceInMetres).Take(3).ToList();


            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(expectedResult[i].Postcode, result[i].Postcode);
                Assert.AreEqual(expectedResult[i].DistanceInMetres, result[i].DistanceInMetres);
            }
        }


        [Test]
        public async Task GetPostcodesFromDb_ResultsAreOrdered_LimitedByRadius()
        {
            _repository.Setup(x => x.GetPreComputedNearestPostcodes(It.IsAny<string>())).ReturnsAsync(default(PreComputedNearestPostcodesDto));

            NearestPostcodeGetter nearestPostcodeGetter = new NearestPostcodeGetter(_repository.Object, _applicationConfig.Object);

            IReadOnlyList<NearestPostcodeDto> result = await nearestPostcodeGetter.GetNearestPostcodesAsync(_postcode, 3, 5);

            List<NearestPostcodeDto> expectedResult = _nearestPostcodeDtos.Where(x => x.DistanceInMetres <= 3).OrderBy(x => x.DistanceInMetres).Take(5).ToList();

            _repository.Verify(x => x.SavePreComputedNearestPostcodes(It.IsAny<PreComputedNearestPostcodesDto>()));


            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(expectedResult[i].Postcode, result[i].Postcode);
                Assert.AreEqual(expectedResult[i].DistanceInMetres, result[i].DistanceInMetres);
            }
        }

        [Test]
        public async Task GetPostcodesFromDb_ResultsAreOrdered_LimitedByDefaultRadius()
        {
            _repository.Setup(x => x.GetPreComputedNearestPostcodes(It.IsAny<string>())).ReturnsAsync(default(PreComputedNearestPostcodesDto));

            _applicationConfigSettings = new ApplicationConfig()
            {
                DefaultMaxNumberOfNearbyPostcodes = 0,
                DefaultNearestPostcodeRadiusInMetres = 3
            };
            _applicationConfig.SetupGet(x => x.Value).Returns(_applicationConfigSettings);

            NearestPostcodeGetter nearestPostcodeGetter = new NearestPostcodeGetter(_repository.Object, _applicationConfig.Object);

            IReadOnlyList<NearestPostcodeDto> result = await nearestPostcodeGetter.GetNearestPostcodesAsync(_postcode, null, 5);


            List<NearestPostcodeDto> expectedResult = _nearestPostcodeDtos.Where(x => x.DistanceInMetres <= 3).OrderBy(x => x.DistanceInMetres).Take(5).ToList();


            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(expectedResult[i].Postcode, result[i].Postcode);
                Assert.AreEqual(expectedResult[i].DistanceInMetres, result[i].DistanceInMetres);
            }
        }


        [Test]
        public async Task GetPostcodesFromDb_ResultsAreOrdered_LimitedByMaxNumberOfResults()
        {
            _repository.Setup(x => x.GetPreComputedNearestPostcodes(It.IsAny<string>())).ReturnsAsync(default(PreComputedNearestPostcodesDto));

            NearestPostcodeGetter nearestPostcodeGetter = new NearestPostcodeGetter(_repository.Object, _applicationConfig.Object);

            IReadOnlyList<NearestPostcodeDto> result = await nearestPostcodeGetter.GetNearestPostcodesAsync(_postcode, 5, 3);


            List<NearestPostcodeDto> expectedResult = _nearestPostcodeDtos.Where(x => x.DistanceInMetres <= 5).OrderBy(x => x.DistanceInMetres).Take(3).ToList();

            _repository.Verify(x => x.SavePreComputedNearestPostcodes(It.IsAny<PreComputedNearestPostcodesDto>()));

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(expectedResult[i].Postcode, result[i].Postcode);
                Assert.AreEqual(expectedResult[i].DistanceInMetres, result[i].DistanceInMetres);
            }
        }


        [Test]
        public async Task GetPostcodesFromDB_ResultsAreOrdered_LimitedByDefaultMaxNumberOfResults()
        {
            _repository.Setup(x => x.GetPreComputedNearestPostcodes(It.IsAny<string>())).ReturnsAsync(default(PreComputedNearestPostcodesDto));

            _applicationConfigSettings = new ApplicationConfig()
            {
                DefaultMaxNumberOfNearbyPostcodes = 3,
                DefaultNearestPostcodeRadiusInMetres = 0
            };
            _applicationConfig.SetupGet(x => x.Value).Returns(_applicationConfigSettings);

            NearestPostcodeGetter nearestPostcodeGetter = new NearestPostcodeGetter(_repository.Object, _applicationConfig.Object);

            IReadOnlyList<NearestPostcodeDto> result = await nearestPostcodeGetter.GetNearestPostcodesAsync(_postcode, 5, null);

            List<NearestPostcodeDto> expectedResult = _nearestPostcodeDtos.Where(x => x.DistanceInMetres <= 5).OrderBy(x => x.DistanceInMetres).Take(3).ToList();


            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(expectedResult[i].Postcode, result[i].Postcode);
                Assert.AreEqual(expectedResult[i].DistanceInMetres, result[i].DistanceInMetres);
            }
        }

        [Test]
        public async Task GetPostcodesFromDb_GetPostcodesWithinARadiusOf16094Metres()
        {
            _repository.Setup(x => x.GetPreComputedNearestPostcodes(It.IsAny<string>())).ReturnsAsync(default(PreComputedNearestPostcodesDto));

            NearestPostcodeGetter nearestPostcodeGetter = new NearestPostcodeGetter(_repository.Object, _applicationConfig.Object);

            IReadOnlyList<NearestPostcodeDto> result = await nearestPostcodeGetter.GetNearestPostcodesAsync(_postcode, 1, 1);

            _repository.Verify(x => x.GetNearestPostcodesAsync(It.Is<string>(y => y == _postcode), It.Is<double>(y => y == 16094)));
        }

        [Test]
        public async Task GetPostcodesFromCache_CantGetResutsWithARadiusGreaterThan16094Metres()
        {
            NearestPostcodeGetter nearestPostcodeGetter = new NearestPostcodeGetter(_repository.Object, _applicationConfig.Object);

            IReadOnlyList<NearestPostcodeDto> result = await nearestPostcodeGetter.GetNearestPostcodesAsync(_postcode, 99999, 5);

            _repository.Verify(x => x.GetNearestPostcodesAsync(It.IsAny<string>(), It.IsAny<double>()), Times.Never);

            List<NearestPostcodeDto> expectedResult = _nearestPostcodeDtos.Where(x => x.DistanceInMetres <= 16094).OrderBy(x => x.DistanceInMetres).Take(5).ToList();
            
            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(expectedResult[i].Postcode, result[i].Postcode);
                Assert.AreEqual(expectedResult[i].DistanceInMetres, result[i].DistanceInMetres);
            }
        }

        [Test]
        public async Task GetPostcodesFromCache_RadiusOfLessThan1SetTo16094Metres()
        {
            NearestPostcodeGetter nearestPostcodeGetter = new NearestPostcodeGetter(_repository.Object, _applicationConfig.Object);

            IReadOnlyList<NearestPostcodeDto> result = await nearestPostcodeGetter.GetNearestPostcodesAsync(_postcode, 0, 5);

            _repository.Verify(x => x.GetNearestPostcodesAsync(It.IsAny<string>(), It.IsAny<double>()), Times.Never);

            List<NearestPostcodeDto> expectedResult = _nearestPostcodeDtos.Where(x => x.DistanceInMetres <= 16094).OrderBy(x => x.DistanceInMetres).Take(5).ToList();


            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(expectedResult[i].Postcode, result[i].Postcode);
                Assert.AreEqual(expectedResult[i].DistanceInMetres, result[i].DistanceInMetres);
            }
        }

    }
}
