namespace AddressService.Repo.EntityFramework.Entities
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Locality { get; set; }
        public string PostCode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Instructions { get; set; }
    }
}
