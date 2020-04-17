using AddressService.Core.Dto;
using AddressService.Core.Extensions;
using AddressService.Core.Interfaces.Repositories;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AddressService.Handlers
{
    public class PostcodeCoordinatesGetter : IPostcodeCoordinatesGetter
    {
        private readonly IRepository _repository;

        private static readonly ConcurrentDictionary<string, CoordinatesDto> _postcodesWithLatitudeAndLongitudes = new ConcurrentDictionary<string, CoordinatesDto>();

        public PostcodeCoordinatesGetter(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyDictionary<string, CoordinatesDto>> GetPostcodeCoordinates(IEnumerable<string> neededPostcodes)
        {
            IEnumerable<string> allMissingPostCodes = neededPostcodes.Where(x => !_postcodesWithLatitudeAndLongitudes.ContainsKey(x));

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();

            // get coordinates from database in concurrent chunks for speed
            if (allMissingPostCodes.Any())
            {
                IEnumerable<IEnumerable<string>> chunkedMissingPostCodes = allMissingPostCodes.ChunkBy(2500);

                List<Task<IEnumerable<PostcodeWithCoordinatesDto>>> missingPostCodeChunkTasks = new List<Task<IEnumerable<PostcodeWithCoordinatesDto>>>();

                foreach (IEnumerable<string> missingPostcodesChunk in chunkedMissingPostCodes)
                {
                    Task<IEnumerable<PostcodeWithCoordinatesDto>> missingPostCodeTask = _repository.GetPostcodeCoordinatesAsync(missingPostcodesChunk);

                    missingPostCodeChunkTasks.Add(missingPostCodeTask);
                }

                while (missingPostCodeChunkTasks.Count > 0)
                {
                    Task<IEnumerable<PostcodeWithCoordinatesDto>> finishedTask = await Task.WhenAny(missingPostCodeChunkTasks);
                    missingPostCodeChunkTasks.Remove(finishedTask);

                    IEnumerable<PostcodeWithCoordinatesDto> missingPostCodeChunk = await finishedTask;

                    foreach (PostcodeWithCoordinatesDto postcode in missingPostCodeChunk)
                    {
                        _postcodesWithLatitudeAndLongitudes.TryAdd(postcode.Postcode, new CoordinatesDto(postcode.Latitude, postcode.Longitude));
                    }
                }
            }

            Dictionary<string, CoordinatesDto> requiredPostcodes = new Dictionary<string, CoordinatesDto>();

            foreach (var neededPostcode in neededPostcodes)
            {
                if (_postcodesWithLatitudeAndLongitudes.TryGetValue(neededPostcode, out var coordinatesDto))
                {
                    requiredPostcodes[neededPostcode] = coordinatesDto;
                }
            }

            sw2.Stop();
            Debug.WriteLine($"Getting missing postcodes took: {sw2.ElapsedMilliseconds}");

            return requiredPostcodes;
        }
    }
}
