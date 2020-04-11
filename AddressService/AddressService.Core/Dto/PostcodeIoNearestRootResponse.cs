using Newtonsoft.Json;
using System.Collections.Generic;

namespace AddressService.Core.Dto
{
    public class PostCodeIoNearestRootResponse
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("result")]
        public List<PostCodeIoNearestResponse> Result { get; set; }
    }
}