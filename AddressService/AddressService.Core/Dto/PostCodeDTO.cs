using System;
using System.Collections.Generic;
using System.Text;

namespace AddressService.Core.Dto
{
    public class PostCodeDTO
    {
        public int Id { get; set; }
        public string PostalCode { get; set; }
        public int ChampionCount { get; set; }
        public int VolunteerCount { get; set; }
        public List<AddressDetailsDTO> AddressDetails { get; set; }
    }
}
