using HelpMyStreet.Utils.Dtos;

namespace AddressService.Core.Dto
{
    public class PostcodeCoordinateDto : ILatitudeLongitude
    {
        public PostcodeCoordinateDto()
        {
        }

        public PostcodeCoordinateDto(string postcode, double latitude, double longitude)
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
