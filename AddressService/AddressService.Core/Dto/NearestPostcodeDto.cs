using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace AddressService.Core.Dto
{
    public class NearestPostcodeDto
    {
        [JsonProperty("a")]
        [DataMember(Name = "a")]
        public string Postcode { get; set; }

        [JsonProperty("b")]
        [DataMember(Name = "b")]
        public int DistanceInMetres { get; set; }

    }
}
