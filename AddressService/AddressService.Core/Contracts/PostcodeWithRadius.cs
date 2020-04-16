using System.Runtime.Serialization;

namespace AddressService.Core.Contracts
{
    public class PostcodeWithRadius
    {
        //[DataMember(Name = "id")]
        public int Id { get; set; }
        //[DataMember(Name = "pc")]
        public string Postcode { get; set; }
        //[DataMember(Name = "r")]
        public int RadiusInMetres { get; set; }
    }
}
