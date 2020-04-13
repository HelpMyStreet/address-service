using AddressService.Core.Services.PostcodeIo;
using HelpMyStreet.Utils.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Interfaces.Repositories;

namespace AddressService.Core.Validation
{
    public class PostcodeValidator : IPostcodeValidator
    {
        private readonly IRegexPostcodeValidator _regexPostcodeValidator;
        private readonly IRepository _repository;
        private readonly ILogger<PostcodeValidator> _logger;

        public PostcodeValidator(IRegexPostcodeValidator regexPostcodeValidator, IRepository repository, ILogger<PostcodeValidator> logger)
        {
            _regexPostcodeValidator = regexPostcodeValidator;
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Validates postcode by checking using Regex.  If this says it's valid, it checks the DB to see if the postcode is active.
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public async Task<bool> IsPostcodeValidAsync(string postcode)
        {
            if (!_regexPostcodeValidator.IsPostcodeValid(postcode))
            {
                return false;
            }

            postcode = PostcodeFormatter.FormatPostcode(postcode);

            try
            {
                bool isPostcodeInDbAndActive = await _repository.IsPostcodeInDbAndActive(postcode);
                return isPostcodeInDbAndActive;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error calling PostcodeIO to validate postcode. Returning that postcode is valid since it passes Regex", ex);
                return true;
            }

        }
    }
}
