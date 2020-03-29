using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers
{
    public class GetVolunteerCountHandler : IRequestHandler<GetVolunteerCountRequest, VolunteerCountResponse>
    {
        private readonly IRepository _repository;

        public GetVolunteerCountHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<VolunteerCountResponse> Handle(GetVolunteerCountRequest request, CancellationToken cancellationToken)
        {
            var response = new VolunteerCountResponse()
            {
                VolunteerCount = 1,
                ChampionCount = 2
            };
            return Task.FromResult(response);
        }
    }
}
