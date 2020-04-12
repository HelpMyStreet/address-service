using MediatR;
using System.ComponentModel.DataAnnotations;

namespace AddressService.Core.Contracts
{
    public class GetNearbyPostcodesWithoutAddressesRequest : IRequest<GetNearbyPostcodesWithoutAddressesResponse>
    {
        [Required]
        public string Postcode { get; set; }
        public int? RadiusInMetres { get; set; }
        public int? MaxNumberOfResults { get; set; }
    }
}
