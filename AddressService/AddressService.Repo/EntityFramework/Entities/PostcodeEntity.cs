using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace AddressService.Repo.EntityFramework.Entities
{
    public class PostcodeEntity
    {
        public int Id { get; set; }
        public string Postcode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Point Coordinates { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<AddressDetailsEntity> AddressDetails { get; set; } = new List<AddressDetailsEntity>();
    }
}

