namespace AddressService.Core.Dto
{
    public class PostcodeWithLatLongDto
    {
        public PostcodeWithLatLongDto()
        {
        }

        public PostcodeWithLatLongDto(string postcode, double latitude, double longitude)
        {
            Postcode = postcode;
            Latitude = latitude;
            Longitude = longitude;
        }

        public string Postcode { get; }
        public double Latitude { get; }
        public double Longitude { get; }

    }
}
