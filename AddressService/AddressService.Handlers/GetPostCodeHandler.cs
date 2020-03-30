using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Core.Utils;
using AddressService.Mappers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Handlers.PostcodeIo;
using AddressService.Handlers.Qas;

namespace AddressService.Handlers
{
    public class GetPostCodeHandler : IRequestHandler<GetPostCodeRequest, PostCodeResponse>
    {
        private readonly IRepository _repository;
        private readonly IPostcodeIoService _postcodeIoService;
        private readonly IQasService _qasService;
        private readonly IQasMapper _qasMapper;

        public GetPostCodeHandler(IRepository repository, IPostcodeIoService postcodeIoService, IQasService qasService, IQasMapper qasMapper)
        {
            _repository = repository;
            _postcodeIoService = postcodeIoService;
            _qasService = qasService;
            _qasMapper = qasMapper;
        }

        public async Task<PostCodeResponse> Handle(GetPostCodeRequest request, CancellationToken cancellationToken)
        {
            string postcode = PostcodeCleaner.CleanPostcode(request.PostCode);

            QasRootResponse qasRootResponse = await _qasService.GetGlobalIntuitiveSearchResponse(postcode);

            PostCodeResponse postCodeResponse = _qasMapper.MapResponse(qasRootResponse);

            return postCodeResponse;
        }
    }
}
