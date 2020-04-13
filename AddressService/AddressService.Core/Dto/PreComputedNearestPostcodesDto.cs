namespace AddressService.Core.Dto
{
    public class PreComputedNearestPostcodesDto
    {
        public int Id { get; set; }
        public string Postcode { get; set; }
        public byte[] CompressedNearestPostcodes { get; set; }
    }
}
