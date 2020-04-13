using Newtonsoft.Json;

namespace AddressService.Core.Dto
{
    public class NearestPostcodeDto
    {
        [JsonProperty("a")]
        public string Postcode { get; set; }

        [JsonProperty("b")]
        public int DistanceInMetres { get; set; }

    }
}
