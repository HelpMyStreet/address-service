using MediatR;
using System.ComponentModel.DataAnnotations;

namespace AddressService.Core.Contracts
{
    public class GetNumberOfAddressesPerPostcodeInBoundaryRequest : IRequest<GetNumberOfAddressesPerPostcodeInBoundaryResponse>
    {
        [Required]
        [Range(-90, 90)]
        public double SWLatitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double SWLongitude { get; set; }

        [Required]
        [Range(-90, 90)]
        public double NELatitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double NELongitude { get; set; }
    }
}
