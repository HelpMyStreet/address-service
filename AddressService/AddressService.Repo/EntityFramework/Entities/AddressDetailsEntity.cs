using System;

namespace AddressService.Repo.EntityFramework.Entities
{
    public class AddressDetailsEntity
    {
        public int Id { get; set; }
        public int PostcodeId { get; set; }
        public virtual PostcodeEntity PostCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Locality { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
