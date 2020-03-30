using System.Collections.Generic;
using Newtonsoft.Json;

namespace AddressService.Core.Dto
{
    public class QasFormatDpvReponse
    {
        [JsonProperty("cmraIndicator")]
        public string CmraIndicator { get; set; }

        [JsonProperty("seedIndicator")]
        public string SeedIndicator { get; set; }

        [JsonProperty("dpvIndicator")]
        public string DpvIndicator { get; set; }

        [JsonProperty("footnotes")]
        public List<string> Footnotes { get; set; }

        [JsonProperty("vacancyIndicator")]
        public string VacancyIndicator { get; set; }

        [JsonProperty("noStatsIndicator")]
        public string NoStatsIndicator { get; set; }

        [JsonProperty("pbsaIndicator")]
        public string PbsaIndicator { get; set; }
    }
}