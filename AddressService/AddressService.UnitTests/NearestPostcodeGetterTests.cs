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

namespace AddressService.UnitTests
{
    public class NearestPostcodeGetterTests
    {
        private Mock<IRepository> _repository;
        private Mock<IOptionsSnapshot<ApplicationConfig>> _applicationConfig;

        private IEnumerable<NearestPostcodeDto> _nearestPostcodeDtos;
        private ApplicationConfig _applicationConfigSettings;
        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IRepository>();

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
            };

            _repository.Setup(x => x.GetNearestPostcodesAsync(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<int>())).ReturnsAsync(_nearestPostcodeDtos);

            _applicationConfigSettings = new ApplicationConfig()
            {
                DefaultMaxNumberOfNearbyPostcodes = 10,
                DefaultNearestPostcodeRadiusInMetres = 10
            };

            _applicationConfig = new Mock<IOptionsSnapshot<ApplicationConfig>>();
            _applicationConfig.SetupGet(x => x.Value).Returns(_applicationConfigSettings);
        }

        [Test]
        public async Task GetPostcodes_DefaultSettings_OrdersResults()
        {
            string postcode = "NG1 5FS";
            NearestPostcodeGetter nearestPostcodeGetter = new NearestPostcodeGetter(_repository.Object, _applicationConfig.Object);

            IReadOnlyList<NearestPostcodeDto> result = await nearestPostcodeGetter.GetNearestPostcodesAsync(postcode, null, null);

            _applicationConfig.SetupGet(x => x.Value).Returns(_applicationConfigSettings);
            _repository.Verify(x => x.GetNearestPostcodesAsync(It.Is<string>(y => y == postcode), It.Is<double>(y => y == (double)_applicationConfigSettings.DefaultNearestPostcodeRadiusInMetres), It.Is<int>(y => y == _applicationConfigSettings.DefaultMaxNumberOfNearbyPostcodes)));

            CollectionAssert.AreEqual(_nearestPostcodeDtos.OrderBy(x => x.DistanceInMetres).ToList(), result.ToList());
        }

        [Test]
        public async Task GetPostcodes_UsesWhatIsRequested()
        {
            string postcode = "NG1 5FS";
            NearestPostcodeGetter nearestPostcodeGetter = new NearestPostcodeGetter(_repository.Object, _applicationConfig.Object);

            IReadOnlyList<NearestPostcodeDto> result = await nearestPostcodeGetter.GetNearestPostcodesAsync(postcode, 2, 1);

            _applicationConfig.SetupGet(x => x.Value).Returns(_applicationConfigSettings);
            _repository.Verify(x => x.GetNearestPostcodesAsync(It.Is<string>(y => y == postcode), It.Is<double>(y => y == (double)2), It.Is<int>(y => y == 1)));

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task GetPostcodes_DoesntAllowRadiusOfMoreThan16094Metres()
        {
            string postcode = "NG1 5FS";
            NearestPostcodeGetter nearestPostcodeGetter = new NearestPostcodeGetter(_repository.Object, _applicationConfig.Object);

            IReadOnlyList<NearestPostcodeDto> result = await nearestPostcodeGetter.GetNearestPostcodesAsync(postcode, 16095, 1);

            _applicationConfig.SetupGet(x => x.Value).Returns(_applicationConfigSettings);
            _repository.Verify(x => x.GetNearestPostcodesAsync(It.Is<string>(y => y == postcode), It.Is<double>(y => y == (double)16094), It.Is<int>(y => y == 1)));
        }
    }
}
