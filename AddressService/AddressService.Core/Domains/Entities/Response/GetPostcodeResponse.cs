using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AddressService.Core.Domains.Entities.Response
{
    [DataContract(Name = "postcode")]
    public class GetPostcodeResponse
    {
        [DataMember(Name = "postcode")]
        public string Postcode { get; set; }

        [DataMember(Name = "addressDetails")]
        public IReadOnlyList<AddressDetailsResponse> AddressDetails { get; set; }

    }
}
