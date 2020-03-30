using System.Collections.Generic;

namespace AddressService.Core.Domains.Entities.Response
{
    public class GetNearbyPostcodesResponse
    {
        public IEnumerable<PostcodeResponse> Postcodes { get; set; } = new List<PostcodeResponse>();
    }
}
