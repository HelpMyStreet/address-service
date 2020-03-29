using AddressService.Core.Domains.Entities.GetPostCode;
using AddressService.Core.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers
{
    public class FunctionAHandler : IRequestHandler<GetPostCodeRequest, GetPostCodeResponse>
    {
        private readonly IRepository _repository;

        public FunctionAHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<GetPostCodeResponse> Handle(GetPostCodeRequest request, CancellationToken cancellationToken)
        {
            _repository.AddPostCode(new Core.Dto.PostCodeDTO()
            {
                PostalCode = "PG"
            });
            var response = new GetPostCodeResponse()
            {
                Status = "Active"
            };
            return Task.FromResult(response);
        }
    }
}
