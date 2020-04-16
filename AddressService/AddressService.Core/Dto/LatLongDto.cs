namespace AddressService.Core.Dto
{
    public class LatLongDto
    {
        public LatLongDto(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; }
        public double Longitude { get; }

    }
}
