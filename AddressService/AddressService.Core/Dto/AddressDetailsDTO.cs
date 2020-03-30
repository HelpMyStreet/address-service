using System;
using System.Collections.Generic;
using System.Text;

namespace AddressService.Core.Dto
{
    public class AddressDetailsDTO
    {
        public int Id { get; set; }
        public string HouseName { get; set; }
        public string HouseNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public int PostcodeId { get; set; }
        public PostcodeDTO Postcode { get; set; }
    }
}
