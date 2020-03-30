using System.Collections.Generic;

namespace AddressService.Core.Domains.Entities.Response
{
    public class PostCodeResponse
    {
        public string PostCode { get; set; }
        public List<string> Addresses { get; set; }

    }
}
