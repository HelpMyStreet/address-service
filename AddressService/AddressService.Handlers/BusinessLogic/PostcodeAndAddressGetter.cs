using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using HelpMyStreet.Utils.Utils;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers.BusinessLogic
{
    public class PostcodeAndAddressGetter : IPostcodeAndAddressGetter
    {
        private readonly IRepository _repository;
        private readonly IQasAddressGetter _qasAddressGetter;

        // cache for postcodes that QAS says doesn't have any addresses so needless calls to QAS aren't made
        private static readonly HashSet<string> _postcodesWithoutAddressesCache = new HashSet<string>();

        public PostcodeAndAddressGetter(IRepository repository, IQasAddressGetter qasAddressGetter)
        {
            _repository = repository;
            _qasAddressGetter = qasAddressGetter;
        }

        public async Task<PostcodeDto> GetPostcodeAsync(string postcode, CancellationToken cancellationToken)
        {
            IEnumerable<PostcodeDto> postcodes = await GetPostcodesAsync(new List<string>() { postcode }, cancellationToken);
            PostcodeDto postcodeDto = postcodes.FirstOrDefault();
            return postcodeDto;
        }


        public async Task<IEnumerable<PostcodeDto>> GetPostcodesAsync(IEnumerable<string> postcodes, CancellationToken cancellationToken)
        {
            // get postcodes from database
            IEnumerable<PostcodeDto> postcodesFromDb = await _repository.GetPostcodesAsync(postcodes);
            ImmutableHashSet<string> postcodesFromDbHashSet = postcodesFromDb.Select(x => x.Postcode).ToImmutableHashSet();

            // find missing postcodes
            List<string> missingPostcodes = postcodes.Where(x => !postcodesFromDbHashSet.Contains(x)).ToList();

            if (!missingPostcodes.Any())
            {
                return postcodesFromDb;
            }

            IEnumerable<PostcodeDto> missingPostcodeDtos = await _qasAddressGetter.GetPostCodesAndAddressesFromQasAsync(missingPostcodes, cancellationToken);

            // add missing postcodes to those originally taken from the DB
            IEnumerable<PostcodeDto> allPostcodeDtos = postcodesFromDb.Concat(missingPostcodeDtos);

            return allPostcodeDtos;
        }

        public async Task<IEnumerable<PostcodeWithNumberOfAddressesDto>> GetNumberOfAddressesPerPostcodeAsync(IEnumerable<string> postcodes, CancellationToken cancellationToken)
        {
            postcodes = postcodes.Select(x => PostcodeFormatter.FormatPostcode(x)).ToList();

            // get postcodes with number of addresses from database
            IEnumerable<PostcodeWithNumberOfAddressesDto> postCodesWithNumberOfAddresses = await _repository.GetNumberOfAddressesPerPostcodeAsync(postcodes);

            ImmutableHashSet<string> postcodesFromDbHashSet = postCodesWithNumberOfAddresses.Select(x => x.Postcode).ToImmutableHashSet();

            // find postcodes without addresses and that aren't in the postcodes without addresses cache
            List<string> postcodesWithoutAddresses = postcodes.Where(x => !postcodesFromDbHashSet.Contains(x) && !_postcodesWithoutAddressesCache.Contains(x)).ToList();
            
            if (!postcodesWithoutAddresses.Any())
            {
                return postCodesWithNumberOfAddresses;
            }

            // get and save addresses for postcodes without addresses
            IEnumerable<PostcodeDto> missingPostcodeDtos = await _qasAddressGetter.GetPostCodesAndAddressesFromQasAsync(postcodesWithoutAddresses, cancellationToken);

            IEnumerable<string> postcodesThatHaveNoAddressess = postcodesWithoutAddresses.Where(x => !missingPostcodeDtos.Select(y => y.Postcode).Contains(x));

            foreach (string postcodeThatHasNoAddress  in postcodesThatHaveNoAddressess)
            {
                _postcodesWithoutAddressesCache.Add(postcodeThatHasNoAddress);
            }

            IEnumerable<PostcodeWithNumberOfAddressesDto> missingPostCodesWithNumberOfAddresses = missingPostcodeDtos.GroupBy(x => x.Postcode)

                .Select(x => new PostcodeWithNumberOfAddressesDto()
                {
                    Postcode = x.Key,
                    NumberOfAddresses = x.Sum(y=>y.AddressDetails.Count)
                });

            IEnumerable<PostcodeWithNumberOfAddressesDto> postCodesWithNumberOfAddresses2 = postCodesWithNumberOfAddresses.Where(x => x.NumberOfAddresses >= 0).Concat(missingPostCodesWithNumberOfAddresses);

            return postCodesWithNumberOfAddresses2;
        }


    }
}

