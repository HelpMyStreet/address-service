using HelpMyStreet.Utils.Dtos;

namespace AddressService.Core.Dto
{
    public class CoordinatesDto : ILatitudeLongitude
    {
        public CoordinatesDto()
        {
        }

        public CoordinatesDto(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; }
        public double Longitude { get;  }

    }
}
