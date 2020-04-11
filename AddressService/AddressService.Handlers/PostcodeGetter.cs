using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Core.Utils;
using AddressService.Mappers;
using AutoMapper;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Services.Qas;
using HelpMyStreet.Utils.Utils;

namespace AddressService.Handlers
{
    public class PostcodeGetter : IPostcodeGetter
    {
        private readonly IRepository _repository;
        private readonly IQasService _qasService;
        private readonly IQasMapper _qasMapper;

        public PostcodeGetter(IRepository repository, IQasService qasService, IQasMapper qasMapper)
        {
            _repository = repository;
            _qasService = qasService;
            _qasMapper = qasMapper;
        }

        public async Task<PostcodeDto> GetPostcodeAsync(string postcode, CancellationToken cancellationToken)
        {
            var postcodes = await GetPostcodesAsync(new List<string>() { postcode }, cancellationToken);
            var postcodeDto = postcodes.FirstOrDefault();
            return postcodeDto;
        }

        public async Task<IEnumerable<PostcodeDto>> GetPostcodesAsync(IEnumerable<string> postcodes, CancellationToken cancellationToken)
        {
            postcodes = postcodes.Select(x => PostcodeFormatter.FormatPostcode(x)).ToList();

            // get postcodes from database
            IEnumerable<PostcodeDto> postcodesFromDb = await _repository.GetPostcodesAsync(postcodes);
            ImmutableHashSet<string> postcodesFromDbHashSet = postcodesFromDb.Select(x => x.Postcode).ToImmutableHashSet();

            // find missing postcodes
            List<string> missingPostcodes = postcodes.Where(x => !postcodesFromDbHashSet.Contains(x)).ToList();

            if (!missingPostcodes.Any())
            {
                return postcodesFromDb;
            }

            // call QAS for missing postcodes and addresses
            List<Task<QasSearchRootResponse>> qasSearchResponseTasks = new List<Task<QasSearchRootResponse>>();
            List<QasSearchRootResponse> qasSearchResponses = new List<QasSearchRootResponse>();

            foreach (string missingPostcode in missingPostcodes)
            {
                Task<QasSearchRootResponse> qasResponseTask = _qasService.GetGlobalIntuitiveSearchResponseAsync(PostcodeFormatter.FormatPostcode(missingPostcode), cancellationToken);
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
            List<PostcodeDto> missingPostcodeDtos = new List<PostcodeDto>();

            foreach (IGrouping<string, string> missingQasFormatIds in missingQasFormatIdsGroupedByPostCode)
            {
                List<Task<QasFormatRootResponse>> qasFormatResponseTasks = new List<Task<QasFormatRootResponse>>();
                foreach (string missingQasFormatId in missingQasFormatIds)
                {
                    Task<QasFormatRootResponse> qasFormatResponseTask = _qasService.GetGlobalIntuitiveFormatResponseAsync(missingQasFormatId, cancellationToken);
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

                PostcodeDto missingPostcodeDtosForThisBatch = _qasMapper.MapToPostcodeDto(missingQasFormatIds.Key, qasFormatResponses);
                missingPostcodeDtos.Add(missingPostcodeDtosForThisBatch);
            }

            if (missingPostcodeDtos.Any())
            {
                await _repository.SaveAddressesAsync(missingPostcodeDtos);
            }
            
            // add missing postcodes to those originally taken from the DB
            IEnumerable<PostcodeDto> allPostcodeDtos = postcodesFromDb.Concat(missingPostcodeDtos);

            return allPostcodeDtos;
        }

    }
}

