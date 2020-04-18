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

            string nearestPostcodesJson = JsonConvert.SerializeObject(nearestPostcodeDtos);

            preComputedNearestPostcodeses.CompressedNearestPostcodes  = CompressionUtils.Zip(nearestPostcodesJson);

            return preComputedNearestPostcodeses;
        }

        public static IReadOnlyList<NearestPostcodeDto> DecompressPreComputedPostcodes(PreComputedNearestPostcodesDto preComputedNearestPostcodesDto)
        {
            string decompressedPreComputedNearbyPostcodesJson = CompressionUtils.Unzip(preComputedNearestPostcodesDto.CompressedNearestPostcodes);

            List<NearestPostcodeDto> nearbyPostcodeDtos = JsonConvert.DeserializeObject<List<NearestPostcodeDto>>(decompressedPreComputedNearbyPostcodesJson);

            return nearbyPostcodeDtos;
        }

    }
}
