using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AddressService.Core.Domains.Entities.Response
{
    [DataContract(Name = "postcode")]
    public class PostcodeResponse
    {
        public string PostCode { get; set; }
        public List<AddressDetailsResponse> AddressDetails { get; set; }

    }
}
