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
    public class PostDecrementVolunteerCountHandler : IRequestHandler<PostDecrementVolunteerCountRequest>
    {
        private readonly IRepository _repository;

        public PostDecrementVolunteerCountHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<Unit> Handle(PostDecrementVolunteerCountRequest request, CancellationToken cancellationToken)
        {
            _repository.DecrementVolunteerCount(request.PostCode);
            return Unit.Task;
        }
    }
}
