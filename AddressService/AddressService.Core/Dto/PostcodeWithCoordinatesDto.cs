namespace AddressService.Core.Dto
{
    public class PostcodeWithCoordinatesDto
    {
        public PostcodeWithCoordinatesDto()
        {
        }

        public PostcodeWithCoordinatesDto(string postcode, double latitude, double longitude)
        {
            Postcode = postcode;
            Latitude = latitude;
            Longitude = longitude;
        }

        public string Postcode { get; }
        public double Latitude { get; }
        public double Longitude { get;  }

    }
}
