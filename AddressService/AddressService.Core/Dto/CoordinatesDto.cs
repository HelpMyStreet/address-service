namespace AddressService.Core.Dto
{
    public class CoordinatesDto
    {
        public CoordinatesDto(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; }
        public double Longitude { get; }

    }
}
