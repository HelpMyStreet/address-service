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
    public class GetPostcodeHandler : IRequestHandler<GetPostcodeRequest, PostcodeResponse>
    {
        private readonly IRepository _repository;
        private readonly IPostcodeIoService _postcodeIoService;
        private readonly IQasService _qasService;
        private readonly IQasMapper _qasMapper;

        public GetPostcodeHandler(IRepository repository, IPostcodeIoService postcodeIoService, IQasService qasService, IQasMapper qasMapper)
        {
            _repository = repository;
            _postcodeIoService = postcodeIoService;
            _qasService = qasService;
            _qasMapper = qasMapper;
        }

        public async Task<PostcodeResponse> Handle(GetPostcodeRequest request, CancellationToken cancellationToken)
        {
            string postcode = PostcodeCleaner.CleanPostcode(request.Postcode);

            QasRootResponse qasRootResponse = await _qasService.GetGlobalIntuitiveSearchResponse(postcode);

            PostcodeResponse postcodeResponse = _qasMapper.MapResponse(qasRootResponse);

            return postcodeResponse;
        }
    }
}
