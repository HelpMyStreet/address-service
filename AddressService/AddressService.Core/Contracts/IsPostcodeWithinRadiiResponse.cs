using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AddressService.Core.Contracts
{
    [DataContract(Name = "isPostcodeWithinRadiiResponse")]
    public class IsPostcodeWithinRadiiResponse
    {
        [DataMember(Name = "idsWithinRadius")]
        public IEnumerable<int> IdsWithinRadius { get; set; }
    }
}
