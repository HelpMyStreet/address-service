using System.Runtime.Serialization;

namespace AddressService.Core.Contracts
{
    [DataContract(Name = "postcodeWithRadius")]
    public class PostcodeWithRadius
    {
        // this has shorter property names to makes the requests smaller

        [DataMember(Name = "id")] 
        public int Id { get; set; }

        [DataMember(Name = "pc")]
        public string Postcode { get; set; }

        [DataMember(Name = "r")]
        public int RadiusInMetres { get; set; }
    }
}
