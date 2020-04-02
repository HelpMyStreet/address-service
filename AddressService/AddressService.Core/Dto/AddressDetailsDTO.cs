namespace AddressService.Core.Dto
{
    public class AddressDetailsDto
    {
        public int Id { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Locality { get; set; }
        public string PostalCode { get; set; }
        public int PostCodeId { get; set; }
        public virtual PostcodeDto PostCode { get; set; }
    }
}
