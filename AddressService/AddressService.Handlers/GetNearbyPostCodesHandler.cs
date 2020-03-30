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
    public class GetNearbyPostcodesHandler : IRequestHandler<GetNearbyPostcodesRequest, GetNearbyPostcodesResponse>
    {
        private readonly IRepository _repository;
        private readonly IPostcodeIoService _postcodeIoService;
        private readonly IQasService _qasService;
        private readonly IQasMapper _qasMapper;

        public GetNearbyPostcodesHandler(IRepository repository, IPostcodeIoService postcodeIoService, IQasService qasService, IQasMapper qasMapper)
        {
            _repository = repository;
            _postcodeIoService = postcodeIoService;
            _qasService = qasService;
            _qasMapper = qasMapper;
        }

        public async Task<GetNearbyPostcodesResponse> Handle(GetNearbyPostcodesRequest request, CancellationToken cancellationToken)
        {
            string postcode = PostcodeCleaner.CleanPostcode(request.Postcode);

            PostCodeIoNearestRootResponse postCodeIoResponse = await _postcodeIoService.GetNearbyPostCodesAsync(postcode);

            List<Task<QasRootResponse>> qasResponseTasks = new List<Task<QasRootResponse>>();
            List<PostcodeResponse> postcodeResponses = new List<PostcodeResponse>();

            GetNearbyPostcodesResponse getNearbyPostcodesResponse = new GetNearbyPostcodesResponse();

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
                PostcodeResponse postcodeResponse = _qasMapper.MapResponse(qasResponse);
                postcodeResponses.Add(postcodeResponse);
            }

            getNearbyPostcodesResponse.Postcodes = postcodeResponses;

            return getNearbyPostcodesResponse;
        }
    }
}
