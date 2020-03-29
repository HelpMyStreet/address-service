using AddressService.Core.Domains.Entities;
using AddressService.Core.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers
{
    public class FunctionAHandler : IRequestHandler<FunctionARequest, FunctionAResponse>
    {
        private readonly IRepository _repository;

        public FunctionAHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<FunctionAResponse> Handle(FunctionARequest request, CancellationToken cancellationToken)
        {
            _repository.AddPostCode(new Core.Dto.PostCodeDTO()
            {
                PostalCode = "PG"
            });
            var response = new FunctionAResponse()
            {
                Status = "Active"
            };
            return Task.FromResult(response);
        }
    }
}
