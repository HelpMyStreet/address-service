using AddressService.Core.Domains.Entities.GetNearbyPostCodes;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers
{
    public class GetNearbyPostCodesHandler : IRequestHandler<GetNearbyPostCodesRequest, GetNearbyPostCodesResponse>
    {
        public Task<GetNearbyPostCodesResponse> Handle(GetNearbyPostCodesRequest request, CancellationToken cancellationToken)
        {
            var response = new GetNearbyPostCodesResponse()
            {
                Status = "Active"
            };
            return Task.FromResult(response);
        }
    }
}
