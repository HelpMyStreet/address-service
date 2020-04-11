using System;
using System.Collections.Generic;

namespace AddressService.Repo.EntityFramework.Entities
{
    public class AddressPostcodeEntity
    {
        public int Id { get; set; }
        public DateTime LastUpdated { get; set; }

        public virtual ICollection<AddressDetailsEntity> AddressDetails { get; set; } = new List<AddressDetailsEntity>();
        public virtual PostcodeEntity PostcodeEntity { get; set; } 
    }
}

