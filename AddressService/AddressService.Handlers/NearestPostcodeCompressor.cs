using AddressService.Core.Dto;
using AddressService.Core.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using AddressService.Core.Utils;

namespace AddressService.Handlers
{
    public class NearestPostcodeCompressor
    {
        public static PreComputedNearestPostcodesDto CompressNearestPostcodeDtos(string postcode, IEnumerable<NearestPostcodeDto> nearestPostcodeDtos)
        {
            PreComputedNearestPostcodesDto preComputedNearestPostcodeses = new PreComputedNearestPostcodesDto();
            preComputedNearestPostcodeses.Postcode = postcode;

            byte[] nearestPostcodesBytes = Utf8Json.JsonSerializer.Serialize(nearestPostcodeDtos);

            preComputedNearestPostcodeses.CompressedNearestPostcodes = CompressionUtils.Zip(nearestPostcodesBytes);

            return preComputedNearestPostcodeses;
        }

        public static IReadOnlyList<NearestPostcodeDto> DecompressPreComputedPostcodes(PreComputedNearestPostcodesDto preComputedNearestPostcodesDto)
        {
            byte[] decompressedPreComputedNearbyPostcodesBytes = CompressionUtils.UnzipToBytes(preComputedNearestPostcodesDto.CompressedNearestPostcodes);

            List<NearestPostcodeDto> nearbyPostcodeDtos = Utf8Json.JsonSerializer.Deserialize<List<NearestPostcodeDto>>(decompressedPreComputedNearbyPostcodesBytes);

            return nearbyPostcodeDtos;
        }

    }
}
