using System.Runtime.Serialization;

namespace AddressService.Core.Contracts
{
    public class PostcodeCoordinate
    {
        [DataMember(Name = "postcode")]
        public string Postcode { get; set; }

        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "long")]
        public double Longitude { get; set; }
    }
}
