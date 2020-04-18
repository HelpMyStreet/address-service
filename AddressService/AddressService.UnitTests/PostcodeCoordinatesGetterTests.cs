using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Handlers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AddressService.UnitTests
{
    public class PostcodeCoordinatesGetterTests
    {
        private Mock<IRepository> _repository;
        private IEnumerable<PostcodeWithCoordinatesDto> _postcodes1;
        private IEnumerable<PostcodeWithCoordinatesDto> _postcodes2;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IRepository>();


            _postcodes1 = new List<PostcodeWithCoordinatesDto>()
            {
                new PostcodeWithCoordinatesDto("NG1 5FS", -1.155263, -1.155263),
                new PostcodeWithCoordinatesDto("NG1 5FW", -1.155413, 52.955491), // 68m
                new PostcodeWithCoordinatesDto("NG1 5BL", -1.154864, 52.955494), // 73m
              
            };

            _postcodes2 = new List<PostcodeWithCoordinatesDto>()
            {
                new PostcodeWithCoordinatesDto("NG1 6LP", -1.154102, 52.954771), //  79m
                new PostcodeWithCoordinatesDto("NG1 6LF", 52.954832, -1.153744), // 102m
                new PostcodeWithCoordinatesDto("NG1 6LA", 52.954446, -1.153885), // 104m

            };

            _repository.SetupSequence(x => x.GetPostcodeCoordinatesAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(_postcodes1)
                .ReturnsAsync(_postcodes2);
        }

        [Test]
        public async Task PostcodeCoordinatesAreCached()
        {
            var postcodeCoordinatesGetter = new PostcodeCoordinatesGetter(_repository.Object);

            var wantedPostCodes1 = new List<string>() { "NG1 5FS", "NG1 5FW", "NG1 5BL" };
            var wantedPostCodes2 = new List<string>() { "NG1 6LP", "NG1 6LF", "NG1 6LA" };

            var result1a = await postcodeCoordinatesGetter.GetPostcodeCoordinatesAsync(wantedPostCodes1);
            var result2a = await postcodeCoordinatesGetter.GetPostcodeCoordinatesAsync(wantedPostCodes2);

            var result1b = await postcodeCoordinatesGetter.GetPostcodeCoordinatesAsync(wantedPostCodes1);
            var result2b = await postcodeCoordinatesGetter.GetPostcodeCoordinatesAsync(wantedPostCodes2);


            _repository.Verify(x => x.GetPostcodeCoordinatesAsync(It.Is<IEnumerable<string>>(y => y.Count() == 3 && y.All(z => wantedPostCodes1.Contains(z)))), Times.Once);


            _repository.Verify(x => x.GetPostcodeCoordinatesAsync(It.Is<IEnumerable<string>>(y => y.Count() == 3 && y.All(z => wantedPostCodes2.Contains(z)))), Times.Once);

            Assert.AreEqual(3, result1a.Count);
            Assert.AreEqual(_postcodes1.FirstOrDefault(x => x.Postcode == "NG1 5FS").Latitude, result1a["NG1 5FS"].Latitude);
            Assert.AreEqual(_postcodes1.FirstOrDefault(x => x.Postcode == "NG1 5FW").Latitude, result1a["NG1 5FW"].Latitude);
            Assert.AreEqual(_postcodes1.FirstOrDefault(x => x.Postcode == "NG1 5BL").Latitude, result1a["NG1 5BL"].Latitude);

            Assert.AreEqual(3, result2a.Count);
            Assert.AreEqual(_postcodes2.FirstOrDefault(x => x.Postcode == "NG1 6LP").Latitude, result2a["NG1 6LP"].Latitude);
            Assert.AreEqual(_postcodes2.FirstOrDefault(x => x.Postcode == "NG1 6LF").Latitude, result2a["NG1 6LF"].Latitude);
            Assert.AreEqual(_postcodes2.FirstOrDefault(x => x.Postcode == "NG1 6LA").Latitude, result2a["NG1 6LA"].Latitude);

            Assert.AreEqual(3, result1b.Count);
            Assert.AreEqual(_postcodes1.FirstOrDefault(x => x.Postcode == "NG1 5FS").Latitude, result1b["NG1 5FS"].Latitude);
            Assert.AreEqual(_postcodes1.FirstOrDefault(x => x.Postcode == "NG1 5FW").Latitude, result1b["NG1 5FW"].Latitude);
            Assert.AreEqual(_postcodes1.FirstOrDefault(x => x.Postcode == "NG1 5BL").Latitude, result1b["NG1 5BL"].Latitude);

            Assert.AreEqual(3, result2b.Count);
            Assert.AreEqual(_postcodes2.FirstOrDefault(x => x.Postcode == "NG1 6LP").Latitude, result2b["NG1 6LP"].Latitude);
            Assert.AreEqual(_postcodes2.FirstOrDefault(x => x.Postcode == "NG1 6LF").Latitude, result2b["NG1 6LF"].Latitude);
            Assert.AreEqual(_postcodes2.FirstOrDefault(x => x.Postcode == "NG1 6LA").Latitude, result2b["NG1 6LA"].Latitude);
        }
    }
}
