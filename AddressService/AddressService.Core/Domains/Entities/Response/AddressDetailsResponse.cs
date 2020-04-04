using System.Runtime.Serialization;

namespace AddressService.Core.Domains.Entities.Response
{
    [DataContract(Name = "addressDetails")]
    public class AddressDetailsResponse
    {
        [DataMember(Name = "addressLine1")]
        public string AddressLine1 { get; set; }

        [DataMember(Name = "addressLine2")]
        public string AddressLine2 { get; set; }

        [DataMember(Name = "addressLine3")]
        public string AddressLine3 { get; set; }

        [DataMember(Name = "locality")]
        public string Locality { get; set; }

        [DataMember(Name = "postcode")]
        public string Postcode { get; set; }
    }
}
