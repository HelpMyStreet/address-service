using AddressService.Core.Dto;
using AddressService.Core.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AddressService.UnitTests
{
    public class FriendlyNameGeneratorTests
    {
        PostcodeDto _postcodeDto;

        [SetUp]
        public void Setup()
        {
            _postcodeDto = new PostcodeDto();
        }

        [Test]
        public void FriendlyNameSimpleAddress()
        {
            _postcodeDto.AddressDetails = new List<AddressDetailsDto>
            {
                new AddressDetailsDto()
                {
                    AddressLine1 = "1 My Street",
                    AddressLine2 = null,
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "2a My Street",
                    AddressLine2 = null,
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "2b My Street",
                    AddressLine2 = null,
                    AddressLine3 =null,
                    Locality = "aaa"
                }
            };

            FriendlyNameGenerator generator = new FriendlyNameGenerator();
            generator.GenerateFriendlyName(_postcodeDto);

            string result = "My Street";
            Assert.AreEqual(result, _postcodeDto.FriendlyName);
        }

        [Test]
        public void FriendlyNameLessSimpleAddress()
        {
            _postcodeDto.AddressDetails = new List<AddressDetailsDto>
            {
                new AddressDetailsDto()
                {
                    AddressLine1 = "1 Midland Cottages",
                    AddressLine2 = "West Bridgeford",
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "2 Midland Cottages",
                    AddressLine2 = "West Bridgeford",
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "3 Midland Cottages",
                    AddressLine2 = "West Bridgeford",
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "4 Midland Cottages",
                    AddressLine2 = "West Bridgeford",
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "5 Midland Cottages",
                    AddressLine2 = "West Bridgeford",
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "6 Midland Cottages",
                    AddressLine2 = "West Bridgeford",
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "7 Midland Cottages",
                    AddressLine2 = "West Bridgeford",
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "8 Midland Cottages",
                    AddressLine2 = "West Bridgeford",
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "9 Midland Cottages",
                    AddressLine2 = "West Bridgeford",
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "10 Midland Cottages",
                    AddressLine2 = "West Bridgeford",
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "11 Midland Cottages",
                    AddressLine2 = "West Bridgeford",
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "12 Midland Cottages",
                    AddressLine2 = "West Bridgeford",
                    AddressLine3 = null,
                    Locality = "aaa"
                },
            };

            FriendlyNameGenerator generator = new FriendlyNameGenerator();
            generator.GenerateFriendlyName(_postcodeDto);

            string result = "Midland Cottages";
            Assert.AreEqual(result, _postcodeDto.FriendlyName);
        }

        [Test]
        public void FriendlyNameOddNumbers()
        {
            _postcodeDto.AddressDetails = new List<AddressDetailsDto>
            {
                new AddressDetailsDto()
                {
                    AddressLine1 = "1 My Street",
                    AddressLine2 = null,
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "3 My Street",
                    AddressLine2 = null,
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "5b My Street",
                    AddressLine2 = null,
                    AddressLine3 =null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "7 My Street",
                    AddressLine2 = null,
                    AddressLine3 =null,
                    Locality = "aaa"
                }
            };

            FriendlyNameGenerator generator = new FriendlyNameGenerator();
            generator.GenerateFriendlyName(_postcodeDto);

            string result = "Odd numbers, My Street";
            Assert.AreEqual(result, _postcodeDto.FriendlyName);
        }

        [Test]
        public void FriendlyEvenNumbersWithRange()
        {
            _postcodeDto.AddressDetails = new List<AddressDetailsDto>
            {
                new AddressDetailsDto()
                {
                    AddressLine1 = "8 My Street",
                    AddressLine2 = null,
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "10 My Street",
                    AddressLine2 = null,
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "12 My Street",
                    AddressLine2 = null,
                    AddressLine3 =null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "14 My Street",
                    AddressLine2 = null,
                    AddressLine3 =null,
                    Locality = "aaa"
                }
            };

            FriendlyNameGenerator generator = new FriendlyNameGenerator();
            generator.GenerateFriendlyName(_postcodeDto);

            string result = "Even numbers, 8-14, My Street";
            Assert.AreEqual(result, _postcodeDto.FriendlyName);
        }

        [Test]
        public void FriendlyNameFlat()
        {
            _postcodeDto.AddressDetails = new List<AddressDetailsDto>
            {
                new AddressDetailsDto()
                {
                    AddressLine1 = "Flat 1",
                    AddressLine2 = "The Building",
                    AddressLine3 = "1 My Street",
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "Flat 2",
                    AddressLine2 = "The Building",
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "Flat 3",
                    AddressLine2 = "The Building",
                    AddressLine3 ="1 My Street",
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "Flat 4",
                    AddressLine2 = "The Building",
                    AddressLine3 ="1 My Street",
                    Locality = "aaa"
                }
            };

            FriendlyNameGenerator generator = new FriendlyNameGenerator();
            generator.GenerateFriendlyName(_postcodeDto);

            string result = "The Building, My Street";
            Assert.AreEqual(result, _postcodeDto.FriendlyName);
        }

        [Test]
        public void FriendlyNameNonResidential()
        {
            _postcodeDto.AddressDetails = new List<AddressDetailsDto>
            {
                new AddressDetailsDto()
                {
                    AddressLine1 = "A Business",
                    AddressLine2 = "24 My Street",
                    AddressLine3 = "West Bridgeford",
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "Pancakes'r'Us",
                    AddressLine2 = "26 My Street",
                    AddressLine3 = "West Bridgeford",
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "Sparkles and Unicorns",
                    AddressLine2 = "28 My Street",
                    AddressLine3 = "West Bridgeford",
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "Better than the rest",
                    AddressLine2 = "30 My Street",
                    AddressLine3 = "West Bridgeford",
                    Locality = "aaa"
                }
            };

            FriendlyNameGenerator generator = new FriendlyNameGenerator();
            generator.GenerateFriendlyName(_postcodeDto);

            string result = "Even numbers, 24-30, My Street";
            Assert.AreEqual(result, _postcodeDto.FriendlyName);
        }

        [Test]
        public void FriendlyNameLine3FailSafe()
        {
            _postcodeDto.AddressDetails = new List<AddressDetailsDto>
            {
                new AddressDetailsDto()
                {
                    AddressLine1 = "A Business",
                    AddressLine2 = "The Business Building",
                    AddressLine3 = "24 My Street",
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "Pancakes'r'Us",
                    AddressLine2 = "House of Batter",
                    AddressLine3 = "26 My Street",
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "Sparkles and Unicorns",
                    AddressLine2 = "Cloudland",
                    AddressLine3 = "28 My Street",
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "Better than all the rest",
                    AddressLine2 = "South Cleveland Garages",
                    AddressLine3 = "32 My Street",
                    Locality = "aaa"
                }
            };

            FriendlyNameGenerator generator = new FriendlyNameGenerator();
            generator.GenerateFriendlyName(_postcodeDto);

            string result = "Even numbers, 24-32, My Street";
            Assert.AreEqual(result, _postcodeDto.FriendlyName);
        }

        [Test]
        public void FriendlyNameCompleteMess()
        {
            _postcodeDto.AddressDetails = new List<AddressDetailsDto>
            {
                new AddressDetailsDto()
                {
                    AddressLine1 = "A Business",
                    AddressLine2 = "24 My Street",
                    AddressLine3 = "The Business Building",
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "26 My Street",
                    AddressLine2 = "Pancakes'R'Us",
                    AddressLine3 = "House of Batter",
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "Sparkles and Unicorns",
                    AddressLine2 = "Cloudland",
                    AddressLine3 = "28 My Street",
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "Better than all the rest",
                    AddressLine2 = "South Cleveland Garages",
                    AddressLine3 = null,
                    Locality = "aaa"
                }
            };

            FriendlyNameGenerator generator = new FriendlyNameGenerator();
            generator.GenerateFriendlyName(_postcodeDto);

            string result = "";
            Assert.AreEqual(result, _postcodeDto.FriendlyName);
        }

        [Test]
        public void FriendlyNameNoNumbers()
        {
            _postcodeDto.AddressDetails = new List<AddressDetailsDto>
            {
                new AddressDetailsDto()
                {
                    AddressLine1 = "My Street",
                    AddressLine2 = null,
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "My Street",
                    AddressLine2 = null,
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "My Street",
                    AddressLine2 = null,
                    AddressLine3 =null,
                    Locality = "aaa"
                },
                new AddressDetailsDto()
                {
                    AddressLine1 = "My Street",
                    AddressLine2 = null,
                    AddressLine3 =null,
                    Locality = "aaa"
                }
            };

            FriendlyNameGenerator generator = new FriendlyNameGenerator();
            generator.GenerateFriendlyName(_postcodeDto);

            string result = "My Street";
            Assert.AreEqual(result, _postcodeDto.FriendlyName);
        }

    }
}
