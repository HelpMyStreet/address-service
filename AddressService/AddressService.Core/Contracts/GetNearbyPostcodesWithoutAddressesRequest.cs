using System;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace AddressService.Core.Contracts
{
    public class GetNearbyPostcodesWithoutAddressesRequest : IRequest<GetNearbyPostcodesWithoutAddressesResponse>
    {
        [Required]
        public string Postcode { get; set; }

        [Range(0, 16094)] // 10 miles
        public int? RadiusInMetres { get; set; }

        public int? MaxNumberOfResults { get; set; }
    }
}
