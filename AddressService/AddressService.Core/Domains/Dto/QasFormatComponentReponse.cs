using Newtonsoft.Json;

namespace AddressService.Core.Dto
{
    public class QasFormatComponentReponse
    {
        [JsonProperty("building1")]
        public string Building1 { get; set; }

        [JsonProperty("organisation1")]
        public string Organisation1 { get; set; }

        [JsonProperty("streetNumber1")]
        public string StreetNumber1 { get; set; }

        [JsonProperty("street1")]
        public string Street1 { get; set; }

        [JsonProperty("locality1")]
        public string Locality1 { get; set; }

        [JsonProperty("postalCode1")]
        public string PostalCode1 { get; set; }

        [JsonProperty("country1")]
        public string Country1 { get; set; }

        [JsonProperty("countryISO1")]
        public string CountryIso1 { get; set; }
    }
}