using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Core.Utils;
using AddressService.Handlers.PostcodeIo;
using AddressService.Handlers.Qas;
using AddressService.Mappers;

namespace AddressService.Handlers
{
    public class GetNearbyPostCodesHandler : IRequestHandler<GetNearbyPostCodesRequest, GetNearbyPostCodesResponse>
    {
        private readonly IRepository _repository;
        private readonly IPostcodeIoService _postcodeIoService;
        private readonly IQasService _qasService;
        private readonly IQasMapper _qasMapper;

        public GetNearbyPostCodesHandler(IRepository repository, IPostcodeIoService postcodeIoService, IQasService qasService, IQasMapper qasMapper)
        {
            _repository = repository;
            _postcodeIoService = postcodeIoService;
            _qasService = qasService;
            _qasMapper = qasMapper;
        }

        public async Task<GetNearbyPostCodesResponse> Handle(GetNearbyPostCodesRequest request, CancellationToken cancellationToken)
        {
            string postcode = PostcodeCleaner.CleanPostcode(request.PostCode);

            PostCodeIoNearestRootResponse postCodeIoResponse = await _postcodeIoService.GetNearbyPostCodesAsync(postcode);

            List<Task<QasRootResponse>> qasResponseTasks = new List<Task<QasRootResponse>>();
            List<PostCodeResponse> postcodeResponses = new List<PostCodeResponse>();

            GetNearbyPostCodesResponse getNearbyPostCodesResponse = new GetNearbyPostCodesResponse();

            foreach (PostCodeIoNearestResponse nearestPostCodeResult in postCodeIoResponse.Result)
            {
                Task<QasRootResponse> qasResponseTask = _qasService.GetGlobalIntuitiveSearchResponse(PostcodeCleaner.CleanPostcode(nearestPostCodeResult.Postcode));
                qasResponseTasks.Add(qasResponseTask);
            }

            while (qasResponseTasks.Count > 0)
            {
                Task<QasRootResponse> finishedQasResponseTask = await Task.WhenAny(qasResponseTasks);
                qasResponseTasks.Remove(finishedQasResponseTask);
                QasRootResponse qasResponse = await finishedQasResponseTask;
                PostCodeResponse postCodeResponse = _qasMapper.MapResponse(qasResponse);
                postcodeResponses.Add(postCodeResponse);
            }

            getNearbyPostCodesResponse.PostCodes = postcodeResponses;

            return getNearbyPostCodesResponse;
        }
    }
}
