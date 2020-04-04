using Newtonsoft.Json;

namespace AddressService.Core.Dto
{
    public class PostCodeIoValidPostcodeRootResponse
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("result")]
        public bool Result { get; set; }
    }
}