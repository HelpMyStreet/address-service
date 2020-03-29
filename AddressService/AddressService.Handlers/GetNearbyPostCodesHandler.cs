using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers
{
    public class GetNearbyPostCodesHandler : IRequestHandler<GetNearbyPostCodesRequest, PostCodeResponse>
    {
        public Task<PostCodeResponse> Handle(GetNearbyPostCodesRequest request, CancellationToken cancellationToken)
        {
            var response = new PostCodeResponse()
            {
                PostCode = request.PostCode,
                VolunteerCount = 1,
                ChampionCount = 2,
                Addresses = new List<Core.Dto.AddressDetailsDTO>()
                {
                    new Core.Dto.AddressDetailsDTO()
                    {
                        HouseName = "Holly Cottage"
                    },
                    new Core.Dto.AddressDetailsDTO()
                    {
                        HouseName = "Arlington House"
                    }
                }
            };

            return Task.FromResult(response);
        }
    }
}
