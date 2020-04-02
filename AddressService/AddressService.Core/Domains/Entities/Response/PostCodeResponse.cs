using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AddressService.Core.Domains.Entities.Response
{
    [DataContract(Name = "postcode")]
    public class PostcodeResponse
    {
        [DataMember(Name = "postcode")]
        public string PostCode { get; set; }

        [DataMember(Name = "addressDetails")]
        public List<AddressDetailsResponse> AddressDetails { get; set; }

    }
}
