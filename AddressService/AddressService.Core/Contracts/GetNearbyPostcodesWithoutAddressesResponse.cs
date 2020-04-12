using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AddressService.Core.Contracts
{
    [DataContract(Name = "getNearbyPostcodesWithoutAddressesResponse")]
    public class GetNearbyPostcodesWithoutAddressesResponse
    {
        [DataMember(Name = "nearestPostcodes")]
        public IReadOnlyList<NearestPostcodeWithoutAddress> NearestPostcodes { get; set; }

    }
}
