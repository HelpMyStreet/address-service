using System.Runtime.Serialization;

namespace AddressService.Core.Contracts
{
    [DataContract(Name = "nearestPostcode")]
    public class NearestPostcodeWithoutAddress
    {
        [DataMember(Name = "postcode")]
        public string Postcode { get; set; }

        [DataMember(Name = "distanceInMetres")]
        public int DistanceInMetres { get; set; }

    }
}
