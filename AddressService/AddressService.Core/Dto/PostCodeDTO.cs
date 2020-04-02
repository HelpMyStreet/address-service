using System;
using System.Collections.Generic;

namespace AddressService.Core.Dto
{
    public class PostcodeDto
    {
        public int Id { get; set; }
        public string Postcode { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<AddressDetailsDto> AddressDetails { get; set; } = new List<AddressDetailsDto>();
    }
}
