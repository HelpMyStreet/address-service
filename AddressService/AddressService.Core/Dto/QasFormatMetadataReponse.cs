using Newtonsoft.Json;

namespace AddressService.Core.Dto
{
    public class QasFormatMetadataReponse
    {
        [JsonProperty("dpv")]
        public QasFormatDpvReponse Dpv { get; set; }
    }
}