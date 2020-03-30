using System.Collections.Generic;

namespace AddressService.Core.Domains.Entities.Response
{
    public class PostcodeResponse
    {
        public string Postcode { get; set; }
        public List<string> Addresses { get; set; }

    }
}
