using AddressService.Core.Dto;
using AddressService.Handlers;
using HelpMyStreet.Contracts.AddressService.Request;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.UnitTests
{
    public class IsPostcodeWithinRadiiHandlerTests
    {
        private Mock<IPostcodeCoordinatesGetter> _postcodeCoordinatesGetter;

        private IReadOnlyDictionary<string, CoordinatesDto> _postcodeCoordinates;
        [SetUp]
        public void SetUp()
        {

            _postcodeCoordinates = new Dictionary<string, CoordinatesDto>()
            {
                { "NG1 5FS", new CoordinatesDto(52.954885, -1.155263)},
                { "NG1 5FW", new CoordinatesDto(52.955491,-1.155413)}, // 68m away from NG1 5FS
                { "NG1 5BL", new CoordinatesDto(52.955494, -1.154864)}, // 73m
                { "NG1 6LP", new CoordinatesDto(52.954771, -1.154102)}, // 79m
                { "NG1 6LF", new CoordinatesDto(52.95483, -1.153744 )}, // 102m
                { "NG1 6LA", new CoordinatesDto(52.954446, -1.153885 )} // 104m

            };

            _postcodeCoordinatesGetter = new Mock<IPostcodeCoordinatesGetter>();
            _postcodeCoordinatesGetter.Setup(x => x.GetPostcodeCoordinatesAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(_postcodeCoordinates);
        }

        [Test]
        public async Task TestCorrectdIdsReturned()
        {
            var isPostcodeWithinRadiiHandler = new IsPostcodeWithinRadiiHandler(_postcodeCoordinatesGetter.Object);

            IsPostcodeWithinRadiiRequest request = new IsPostcodeWithinRadiiRequest()
            {
                Postcode = "NG1 5FS",
                PostcodeWithRadiuses = new List<PostcodeWithRadius>()
                {
                    new PostcodeWithRadius()
                    {
                        Id = 1,
                        Postcode = "NG1 5BL",
                        RadiusInMetres = 74
                    },
                    new PostcodeWithRadius()
                    {
                        Id = 2,
                        Postcode = "NG1 6LP",
                        RadiusInMetres = 80
                    },
                    new PostcodeWithRadius()
                    {
                        Id = 3,
                        Postcode = "NG1 6LF",
                        RadiusInMetres = 101
                    },
                    new PostcodeWithRadius()
                    {
                        Id = 4,
                        Postcode = "NG1 6LA",
                        RadiusInMetres = 103
                    }
                },
            };


            IsPostcodeWithinRadiiResponse result = await isPostcodeWithinRadiiHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(2, result.IdsWithinRadius.Count());
            Assert.IsTrue(result.IdsWithinRadius.Contains(1));
            Assert.IsTrue(result.IdsWithinRadius.Contains(2));
        }

    }
}
