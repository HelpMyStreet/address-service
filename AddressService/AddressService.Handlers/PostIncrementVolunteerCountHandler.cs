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
    public class PostIncrementVolunteerCountHandler : IRequestHandler<PostIncrementVolunteerCountRequest>
    {
        private readonly IRepository _repository;

        public PostIncrementVolunteerCountHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<Unit> Handle(PostIncrementVolunteerCountRequest request, CancellationToken cancellationToken)
        {
            _repository.IncrementVolunteerCount(request.PostCode);
            return Unit.Task;
        }
    }
}
