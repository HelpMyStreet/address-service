using System;
using System.Collections.Generic;
using System.Drawing;

namespace AddressService.Core.Dto
{
    public class PostcodeDto
    {
        public int Id { get; set; }
        public string Postcode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Point Coordinates { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool IsActive { get; set; }

        public List<AddressDetailsDto> AddressDetails { get; set; } = new List<AddressDetailsDto>();

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Postcode)}: {Postcode}, {nameof(LastUpdated)}: {LastUpdated}, {nameof(AddressDetails)}: {AddressDetails}";
        }
    }
}
