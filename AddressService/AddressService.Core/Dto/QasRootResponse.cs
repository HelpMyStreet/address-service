using System.Collections.Generic;
using Newtonsoft.Json;

namespace AddressService.Core.Dto
{
    public class QasRootResponse
    {
        [JsonIgnore]
        public string Postcode { get; set; }

        [JsonProperty("totalMatches")]
        public int TotalMatches { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("results")]
        public List<QasResult> Results { get; set; }
    }
}