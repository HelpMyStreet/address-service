using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;
using MediatR;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Core.Utils;
using AddressService.Handlers.PostcodeIo;
using AddressService.Handlers.Qas;
using AddressService.Mappers;
using AutoMapper;

namespace AddressService.Handlers
{
    public class GetNearbyPostcodesHandler : IRequestHandler<GetNearbyPostcodesRequest, GetNearbyPostcodesResponse>
    {
        private readonly IRepository _repository;
        private readonly IPostcodeIoService _postcodeIoService;
        private readonly IQasService _qasService;
        private readonly IQasMapper _qasMapper;
        private readonly IMapper _mapper;

        public GetNearbyPostcodesHandler(IRepository repository, IPostcodeIoService postcodeIoService, IQasService qasService, IQasMapper qasMapper, IMapper mapper)
        {
            _repository = repository;
            _postcodeIoService = postcodeIoService;
            _qasService = qasService;
            _qasMapper = qasMapper;
            _mapper = mapper;
        }

        public async Task<GetNearbyPostcodesResponse> Handle(GetNearbyPostcodesRequest request, CancellationToken cancellationToken)
        {
            // call Postcode IO for nearest postcodes
            string postcode = PostcodeCleaner.CleanPostcode(request.Postcode);

            PostCodeIoNearestRootResponse postCodeIoResponse = await _postcodeIoService.GetNearbyPostCodesAsync(postcode);

            ImmutableHashSet<string> nearestPostcodes = postCodeIoResponse.Result.OrderBy(x => x.Distance).Select(x => PostcodeCleaner.CleanPostcode(x.Postcode)).ToImmutableHashSet();

            // get postcodes from database
            IEnumerable<PostcodeDto> postcodesFromDb = await _repository.GetPostcodes(nearestPostcodes);
            ImmutableHashSet<string> postcodesFromDbHashSet = postcodesFromDb.Select(x => x.Postcode).ToImmutableHashSet();

            // find missing postcodes
            List<string> missingPostcodes = nearestPostcodes.Where(x => !postcodesFromDbHashSet.Contains(x)).ToList();

            // call QAS for missing postcodes and addresses
            List<Task<QasSearchRootResponse>> qasSearchResponseTasks = new List<Task<QasSearchRootResponse>>();
            List<QasSearchRootResponse> qasSearchResponses = new List<QasSearchRootResponse>();

            foreach (string missingPostcode in missingPostcodes)
            {
                Task<QasSearchRootResponse> qasResponseTask = _qasService.GetGlobalIntuitiveSearchResponseAsync(PostcodeCleaner.CleanPostcode(missingPostcode));
                qasSearchResponseTasks.Add(qasResponseTask);
            }

            while (qasSearchResponseTasks.Count > 0)
            {
                Task<QasSearchRootResponse> finishedQasResponseTask = await Task.WhenAny(qasSearchResponseTasks);
                qasSearchResponseTasks.Remove(finishedQasResponseTask);
                QasSearchRootResponse qasSearchResponse = await finishedQasResponseTask;
                qasSearchResponses.Add(qasSearchResponse);
            }

            // call QAS for address details (grouped by postcode to avoid sending 1000s of request at once and to map a single PostcodeDto at a time)
            ILookup<string, string> missingQasFormatIdsGroupedByPostCode = _qasMapper.GetFormatIds(qasSearchResponses);
            List<PostcodeDto> postcodeDtos = new List<PostcodeDto>();

            foreach (IGrouping<string, string> missingQasFormatIds in missingQasFormatIdsGroupedByPostCode)
            {
                List<Task<QasFormatRootResponse>> qasFormatResponseTasks = new List<Task<QasFormatRootResponse>>();
                foreach (string missingQasFormatId in missingQasFormatIds)
                {
                    Task<QasFormatRootResponse> qasFormatResponseTask = _qasService.GetGlobalIntuitiveFormatResponseAsync(missingQasFormatId);
                    qasFormatResponseTasks.Add(qasFormatResponseTask);
                }

                List<QasFormatRootResponse> qasFormatResponses = new List<QasFormatRootResponse>();

                while (qasFormatResponseTasks.Count > 0)
                {
                    Task<QasFormatRootResponse> finishedQasFormatResponseTask = await Task.WhenAny(qasFormatResponseTasks);
                    qasFormatResponseTasks.Remove(finishedQasFormatResponseTask);
                    QasFormatRootResponse qasFormatResponse = await finishedQasFormatResponseTask;
                    qasFormatResponses.Add(qasFormatResponse);
                }

                PostcodeDto missingPostcodeDtos = _qasMapper.MapToPostcodeDto(missingQasFormatIds.Key, qasFormatResponses);
                postcodeDtos.Add(missingPostcodeDtos);
            }

            await _repository.SavePostcodes(postcodeDtos);

            // add missing postcodes to those from DB
            IEnumerable<PostcodeDto> allPostcodeDtos = postcodesFromDb.Concat(postcodeDtos);

            // create response
            GetNearbyPostcodesResponse getNearbyPostcodesResponse = new GetNearbyPostcodesResponse();
            getNearbyPostcodesResponse.Postcodes = _mapper.Map<IEnumerable<PostcodeDto>, IEnumerable<PostcodeResponse>>(allPostcodeDtos);

            foreach (var postcodeResponse in getNearbyPostcodesResponse.Postcodes)
            {
                foreach (var addressDetails in postcodeResponse.AddressDetails)
                {
                    addressDetails.Postcode = postcodeResponse.PostCode;
                }
            }

            return getNearbyPostcodesResponse;
        }
    }
}
