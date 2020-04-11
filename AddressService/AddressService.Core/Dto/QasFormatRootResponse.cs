using Newtonsoft.Json;
using System.Collections.Generic;

namespace AddressService.Core.Dto
{
    public class QasFormatRootResponse
    {
        [JsonProperty("address")]
        public List<QasFormatAddressReponse> Address { get; set; }

        [JsonProperty("components")]
        public List<QasFormatComponentReponse> Components { get; set; }

        [JsonProperty("metadata")]
        public QasFormatMetadataReponse Metadata { get; set; }
    }
}
