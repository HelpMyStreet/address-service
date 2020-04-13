using AddressService.Core.Dto;
using AddressService.Handlers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AddressService.UnitTests
{
    public class NearestPostcodeCompressorTests
    {

        [Test]
        public void TestCompressionAndDecompression()
        {
            var postcode = "NG1 5FS";

            IEnumerable<NearestPostcodeDto> nearestPostcodeDtos = new List<NearestPostcodeDto>(){

            new NearestPostcodeDto()
            {
                Postcode = "NG1 1AA",
                DistanceInMetres = 1

            },
            new NearestPostcodeDto()
            {
                Postcode = "NG1 1AB",
                DistanceInMetres = 2

            }
            };

            var compressedPostcodes = NearestPostcodeCompressor.CompressNearestPostcodeDtos(postcode, nearestPostcodeDtos);

            var decompressedPostcodes = NearestPostcodeCompressor.DecompressPreComputedPostcodes(compressedPostcodes);

            Assert.AreEqual(2, decompressedPostcodes.Count);
            Assert.AreEqual(1, decompressedPostcodes.FirstOrDefault(x => x.Postcode == "NG1 1AA").DistanceInMetres);
            Assert.AreEqual(2, decompressedPostcodes.FirstOrDefault(x => x.Postcode == "NG1 1AB").DistanceInMetres);
        }
    }
}
