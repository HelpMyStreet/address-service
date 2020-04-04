using System.Collections.Generic;
using Newtonsoft.Json;

namespace AddressService.Core.Dto
{
    public class QasSearchResponse
    {
        [JsonProperty("suggestion")]
        public string Suggestion { get; set; }

        [JsonProperty("matched")]
        public List<List<int>> Matched { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }
    }
}