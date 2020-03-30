using AddressService.Core.Domains.Entities.Response;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace AddressService.Core.Domains.Entities.Request
{
    public class GetPostcodeRequest : IRequest<PostcodeResponse>
    {
        [Required]
        [RegularExpression(Validation.PostcodeRegex, ErrorMessage = "Invalid Postcode")]
        public string Postcode { get; set; }
    }
}
