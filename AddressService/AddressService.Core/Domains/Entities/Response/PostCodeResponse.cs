using AddressService.Core.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddressService.Core.Domains.Entities.Response
{
    public class PostCodeResponse
    {
        public string PostCode { get; set; }
        public List<AddressDetailsDTO> Addresses { get; set; }
        public int ChampionCount { get; set; }
        public int VolunteerCount { get; set; }

    }
}
