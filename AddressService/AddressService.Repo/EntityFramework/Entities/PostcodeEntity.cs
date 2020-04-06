using System;
using System.Collections.Generic;

namespace AddressService.Repo.EntityFramework.Entities
{
    public class PostcodeEntity
    {
        public PostcodeEntity()
        {
            AddressDetails = new HashSet<AddressDetailsEntity>();
        }

        public int Id { get; set; }
        public string Postcode { get; set; }
        public DateTime LastUpdated { get; set; }

        public virtual ICollection<AddressDetailsEntity> AddressDetails { get; set; }
    }
}

