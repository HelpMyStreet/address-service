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
    public class GetPostCodeHandler : IRequestHandler<GetPostCodeRequest, PostCodeResponse>
    {
        private readonly IRepository _repository;

        public GetPostCodeHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<PostCodeResponse> Handle(GetPostCodeRequest request, CancellationToken cancellationToken)
        {
            var response = new PostCodeResponse()
            {
                PostCode = request.PostCode,
                Addresses = new List<Core.Dto.AddressDetailsDTO>()
                {
                    new Core.Dto.AddressDetailsDTO()
                    {
                        HouseName = "Holly Cottage"
                    }
                }
            };
            return Task.FromResult(response);
        }
    }
}
