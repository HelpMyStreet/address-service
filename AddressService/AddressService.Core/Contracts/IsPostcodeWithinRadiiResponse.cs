using System.Collections.Generic;

namespace AddressService.Core.Contracts
{
    public class IsPostcodeWithinRadiiResponse
    {
        //public IEnumerable<PostcodeWithRadiusResult> PostcodeWithRadiusResults { get; set; }
        public IEnumerable<int> IdsWithinRadius { get; set; }
    }
}
