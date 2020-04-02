using AddressService.Repo.EntityFramework.Entities.AddressService.Repo.EntityFramework.Entities;

namespace AddressService.Repo.EntityFramework.Entities
{
    public class AddressDetailsEntity
    {
        public int Id { get; set; }
        public int PostCodeId { get; set; }
        public virtual PostcodeEntity PostCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Locality { get; set; }
    }
}
