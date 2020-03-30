using AddressService.Core.Domains.Entities.Response;
using MediatR;

namespace AddressService.Core.Domains.Entities.Request
{
    public class GetNearbyPostCodesRequest : IRequest<GetNearbyPostCodesResponse>
    {
        public string PostCode { get; set; }
    }
}
