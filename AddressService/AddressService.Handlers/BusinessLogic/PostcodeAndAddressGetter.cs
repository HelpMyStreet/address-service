using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using HelpMyStreet.Utils.Utils;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers.BusinessLogic
{
    public class PostcodeAndAddressGetter : IPostcodeAndAddressGetter
    {
        private readonly IRepository _repository;
        private readonly IQasAddressGetter _qasAddressGetter;
        private readonly IPostcodesWithoutAddressesCache _postcodesWithoutAddressesCache;


        public PostcodeAndAddressGetter(IRepository repository, IQasAddressGetter qasAddressGetter, IPostcodesWithoutAddressesCache postcodesWithoutAddressesCache)
        {
            _repository = repository;
            _qasAddressGetter = qasAddressGetter;
            _postcodesWithoutAddressesCache = postcodesWithoutAddressesCache;
        }

        public async Task<PostcodeDto> GetPostcodeAsync(string postcode, CancellationToken cancellationToken)
        {
            IEnumerable<PostcodeDto> postcodes = await GetPostcodesAsync(new List<string>() { postcode }, cancellationToken);
            PostcodeDto postcodeDto = postcodes.FirstOrDefault();
            return postcodeDto;
        }


        public async Task<IEnumerable<PostcodeDto>> GetPostcodesAsync(IEnumerable<string> postcodes, CancellationToken cancellationToken)
        {
            // format postcodes
            postcodes = postcodes.Select(x => PostcodeFormatter.FormatPostcode(x)).ToList();
            
            // get postcodes from database
            IEnumerable<PostcodeDto> postcodesFromDb = await _repository.GetPostcodesAsync(postcodes);

            // find missing postcodes
            ImmutableHashSet<string> postcodesFromDbHashSet = postcodesFromDb.Select(x => x.Postcode).ToImmutableHashSet();
            IEnumerable<string> missingPostcodes = postcodes.Where(x => !postcodesFromDbHashSet.Contains(x)).ToList();

            if (!missingPostcodes.Any())
            {
                return postcodesFromDb;
            }
            
            // filter out postcodes that have no addresses so needless QAS calls aren't made
            missingPostcodes = missingPostcodes.Where(x => !_postcodesWithoutAddressesCache.PostcodesWithoutAddresses.Contains(x));

            IEnumerable<PostcodeDto> missingPostcodeDtos = await _qasAddressGetter.GetPostCodesAndAddressesFromQasAsync(missingPostcodes, cancellationToken);

            // add postcodes that QAS says have no addresses to cache to postcodes without addresses cache
            HashSet<string> postcodesFromQas = missingPostcodeDtos.Select(x => x.Postcode).ToHashSet();
            IEnumerable<string> postcodesThatHaveNoAddressess = missingPostcodes.Where(x => !postcodesFromQas.Contains(x));
            _postcodesWithoutAddressesCache.AddRange(postcodesThatHaveNoAddressess);

            // add missing postcodes to those originally taken from the DB
            IEnumerable<PostcodeDto> allPostcodeDtos = postcodesFromDb.Concat(missingPostcodeDtos);

            return allPostcodeDtos;
        }

        public async Task<IEnumerable<PostcodeWithNumberOfAddressesDto>> GetNumberOfAddressesPerPostcodeAsync(IEnumerable<string> postcodes, CancellationToken cancellationToken)
        {
            // format postcodes
            postcodes = postcodes.Select(x => PostcodeFormatter.FormatPostcode(x)).ToList();

            // filter out postcodes that have no addresses
            postcodes = postcodes.Where(x => !_postcodesWithoutAddressesCache.PostcodesWithoutAddresses.Contains(x));

            // get postcodes with number of addresses from database
            IEnumerable<PostcodeWithNumberOfAddressesDto> postCodesWithNumberOfAddresses = await _repository.GetNumberOfAddressesPerPostcodeAsync(postcodes);

            // find missing postcodes
            ImmutableHashSet<string> postcodesFromDbHashSet = postCodesWithNumberOfAddresses.Select(x => x.Postcode).ToImmutableHashSet();
            List<string> postcodesWithoutAddresses = postcodes.Where(x => !postcodesFromDbHashSet.Contains(x)).ToList();

            if (!postcodesWithoutAddresses.Any())
            {
                return postCodesWithNumberOfAddresses;
            }

            // get and save addresses for postcodes without addresses
            IEnumerable<PostcodeDto> missingPostcodeDtos = await _qasAddressGetter.GetPostCodesAndAddressesFromQasAsync(postcodesWithoutAddresses, cancellationToken);

            // add postcodes that QAS says have no addresses to cache
            HashSet<string> postcodesFromQas = missingPostcodeDtos.Select(x => x.Postcode).ToHashSet();
            IEnumerable<string> postcodesThatHaveNoAddressess = postcodesWithoutAddresses.Where(x => !postcodesFromQas.Contains(x));
            _postcodesWithoutAddressesCache.AddRange(postcodesThatHaveNoAddressess);


            IEnumerable<PostcodeWithNumberOfAddressesDto> missingPostCodesWithNumberOfAddresses = missingPostcodeDtos.GroupBy(x => x.Postcode)

                .Select(x => new PostcodeWithNumberOfAddressesDto()
                {
                    Postcode = x.Key,
                    NumberOfAddresses = x.Sum(y => y.AddressDetails.Count)
                });

            IEnumerable<PostcodeWithNumberOfAddressesDto> postCodesWithNumberOfAddresses2 = postCodesWithNumberOfAddresses.Where(x => x.NumberOfAddresses >= 0).Concat(missingPostCodesWithNumberOfAddresses);

            return postCodesWithNumberOfAddresses2;
        }


    }
}

