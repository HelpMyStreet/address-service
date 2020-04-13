namespace AddressService.Repo.EntityFramework.Entities
{
    public class PreComputedNearestPostcodesEntity
    {
        public int Id { get; set; }
        public string Postcode { get; set; }
        public byte[] CompressedNearestPostcodes { get; set; }
    }
}
