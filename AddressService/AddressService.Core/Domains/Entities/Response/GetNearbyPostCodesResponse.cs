using System.Collections.Generic;

namespace AddressService.Core.Domains.Entities.Response
{
    public class GetNearbyPostCodesResponse
    {
        public IEnumerable<PostCodeResponse> PostCodes { get; set; } = new List<PostCodeResponse>();
    }
}
