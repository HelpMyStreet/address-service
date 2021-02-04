using AddressService.Core.Dto;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Utils;
using AddressService.Handlers.BusinessLogic;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Utils.Utils;
using AddressService.Core.Interfaces.Repositories;

namespace AddressService.Handlers
{
    public class GetLocationsHandler : IRequestHandler<GetLocationsRequest, GetLocationsResponse>
    {
        private readonly IRepository _repository;
        
        public GetLocationsHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetLocationsResponse> Handle(GetLocationsRequest request, CancellationToken cancellationToken)
        {
            return new GetLocationsResponse()
            {
                LocationDetails = await _repository.GetLocations(request.LocationRequests)
            };
        }
    }
}
