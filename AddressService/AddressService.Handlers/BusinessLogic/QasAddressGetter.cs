using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Core.Services.Qas;
using AddressService.Core.Utils;
using AddressService.Mappers;
using HelpMyStreet.Utils.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers.BusinessLogic
{
    public class QasAddressGetter : IQasAddressGetter
    {

        private readonly IRepository _repository;
        private readonly IQasService _qasService;
        private readonly IQasMapper _qasMapper;
        private readonly IFriendlyNameGenerator _friendlyNameGenerator;
        private readonly ILoggerWrapper<QasAddressGetter> _logger;

        public QasAddressGetter(IRepository repository, IQasService qasService, IQasMapper qasMapper, IFriendlyNameGenerator friendlyNameGenerator, ILoggerWrapper<QasAddressGetter> logger)
        {
            _repository = repository;
            _qasService = qasService;
            _qasMapper = qasMapper;
            _friendlyNameGenerator = friendlyNameGenerator;
            _logger = logger;
        }

        public async Task<IEnumerable<PostcodeDto>> GetPostCodesAndAddressesFromQasAsync(IEnumerable<string> missingPostcodes, CancellationToken cancellationToken)
        {
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

                try
                {
                    //new addresses need a friendly name
                    _friendlyNameGenerator.GenerateFriendlyName(missingPostcodeDtosForThisBatch);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Error generating friendly name", ex);
                }

                missingPostcodeDtos.Add(missingPostcodeDtosForThisBatch);
            }

            if (missingPostcodeDtos.Any())
            {
                await _repository.SaveAddressesAndFriendlyNameAsync(missingPostcodeDtos);
            }

            return missingPostcodeDtos;
        }
    }
}
