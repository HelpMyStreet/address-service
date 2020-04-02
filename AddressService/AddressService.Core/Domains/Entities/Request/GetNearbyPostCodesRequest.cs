using System.ComponentModel.DataAnnotations;
using AddressService.Core.Domains.Entities.Response;
using MediatR;

namespace AddressService.Core.Domains.Entities.Request
{
    public class GetNearbyPostcodesRequest : IRequest<GetNearbyPostcodesResponse>
    {
        [Required]
        [RegularExpression(Validation.PostcodeRegex, ErrorMessage = "Invalid Postcode")]
        public string Postcode { get; set; }
    }
}
