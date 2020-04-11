using Newtonsoft.Json;

namespace AddressService.Core.Dto
{
    public class PostCodeIoNearestResponse
    {
        [JsonProperty("postcode")]
        public string Postcode { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }
    }
}