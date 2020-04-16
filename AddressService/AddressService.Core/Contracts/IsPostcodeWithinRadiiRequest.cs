using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AddressService.Core.Contracts
{
    public class IsPostcodeWithinRadiiRequest : IRequest<IsPostcodeWithinRadiiResponse>
    {
        public string Postcode { get; set; }
        [Required]

        //[DataMember(Name = "PostcodeWithRadiuses")]
        public List<PostcodeWithRadius> PostcodeWithRadiuses { get; set; }

    }
}
