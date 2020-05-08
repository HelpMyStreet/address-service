using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Handlers.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers.Cache
{
    public class PostcodeCache : IPostcodeCache
    {
        private readonly IRepository _repository;
        private readonly IBatchedDataGetter _batchedDataGetter;

        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private static IReadOnlyDictionary<string, PostcodeCoordinateDto> _postcodesWithLatitudeAndLongitudes;

        public PostcodeCache(IRepository repository, IBatchedDataGetter batchedDataGetter)
        {
            _repository = repository;
            _batchedDataGetter = batchedDataGetter;
        }

        public async Task<IReadOnlyDictionary<string, PostcodeCoordinateDto>> GetAllPostcodeCoordinatesAsync()
        {
            if (_postcodesWithLatitudeAndLongitudes == null)
            {
                try
                {
                    await _lock.WaitAsync();

                    if (_postcodesWithLatitudeAndLongitudes == null)
                    {
                        IEnumerable<PostcodeCoordinateDto> postcodesWithLatitudeAndLongitudes = await GetAllPostcodeCoordinatesFromDbAsync();

                        _postcodesWithLatitudeAndLongitudes = postcodesWithLatitudeAndLongitudes.ToImmutableDictionary(x => x.Postcode, x => new PostcodeCoordinateDto(x.Postcode, x.Latitude, x.Longitude));
                    }
                }
                finally
                {
                    _lock.Release();
                }
            }

            return _postcodesWithLatitudeAndLongitudes;
        }

        private async Task<IEnumerable<PostcodeCoordinateDto>> GetAllPostcodeCoordinatesFromDbAsync()
        {
            int totalPostcodeCount = await _repository.GetNumberOfPostcodesAsync();

            if (totalPostcodeCount == 0)
            {
                throw new Exception("Please load postcodes using AddressService.PostcodeLoader.  There are instructions in the user guide.");
            }

            int batchSize = 100_000;

            // can't make concurrent calls with Entity Framework without a factory that recreates DbContext for each call
            int minPostcodeId = await _repository.GetMinPostcodeIdAsync();
            int maxPostcodeId = await _repository.GetMaxPostcodeIdAsync();

            IEnumerable<PostcodeCoordinateDto> postcodes = await _batchedDataGetter.GetAsync(async (fromId, toId) => await _repository.GetPostcodeCoordinatesAsync(fromId, toId), minPostcodeId, maxPostcodeId, batchSize);

            return postcodes;
        }

    }
}
