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
    public class GetLocationHandler : IRequestHandler<GetLocationRequest, GetLocationResponse>
    {
        private readonly IRepository _repository;
        
        public GetLocationHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetLocationResponse> Handle(GetLocationRequest request, CancellationToken cancellationToken)
        {
            return new GetLocationResponse()
            {
                LocationDetails = await _repository.GetLocationDetails(request.LocationRequest.Location)
            };
        }
    }
}
