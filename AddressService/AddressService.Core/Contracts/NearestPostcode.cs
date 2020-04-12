using System.Runtime.Serialization;

namespace AddressService.Core.Contracts
{
    [DataContract(Name = "nearestPostcode")]
    public class NearestPostcode
    {
        [DataMember(Name = "postcode")]
        public string Postcode { get; set; }

        [DataMember(Name = "distanceInMetres")]
        public int DistanceInMetres { get; set; }

    }
}
