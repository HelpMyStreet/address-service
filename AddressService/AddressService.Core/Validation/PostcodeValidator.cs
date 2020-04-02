using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Core.Services.PostcodeIo;
using AddressService.Core.Utils;

namespace AddressService.Core.Validation
{
    public class PostcodeValidator
    {
        private readonly IPostcodeIoService _postcodeIoService;
        private readonly IRepository _repository;

        private static readonly Regex _postCodeRegex = new Regex(Validation.PostcodeRegex, RegexOptions.Compiled);

        public PostcodeValidator(IPostcodeIoService postcodeIoService, IRepository repository)
        {
            _postcodeIoService = postcodeIoService;
            _repository = repository;
        }

        public async Task<bool> IsPostcodeValidAsync(string postcode)
        {
            if (String.IsNullOrWhiteSpace(postcode))
            {
                return false;
            }

            postcode = PostcodeCleaner.CleanPostcode(postcode);

            if (!_postCodeRegex.IsMatch(postcode))
            {
                return false;
            }

            bool isInDb = await _repository.IsPostcodeInDb(postcode);

            if (isInDb)
            {
                return true;
            }

            bool doesPostcodeIOThinkPostcodeIsValid = await _postcodeIoService.IsPostcodeValidAsync(postcode, CancellationToken.None);

            return doesPostcodeIOThinkPostcodeIsValid;
        }
    }
}
