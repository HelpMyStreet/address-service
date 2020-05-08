using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AddressService.Core.Contracts
{
    [DataContract(Name = "getNumberOfAddressesInBoundaryResponse")]
    public class GetNumberOfAddressesPerPostcodeInBoundaryResponse
    {
        [DataMember(Name = "postcodesWithNumberOfAddresses")]
        public IReadOnlyList<PostcodeWithNumberOfAddresses> PostcodesWithNumberOfAddresses { get; set; }
    }
}
