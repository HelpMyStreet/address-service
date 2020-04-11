using AddressService.Core.Services.PostcodeIo;
using HelpMyStreet.Utils.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Core.Validation
{
    public class PostcodeValidator : IPostcodeValidator
    {
        private readonly IRegexPostcodeValidator _regexPostcodeValidator;
        private readonly IPostcodeIoService _postcodeIoService;
        private readonly ILogger<PostcodeValidator> _logger;

        public PostcodeValidator(IRegexPostcodeValidator regexPostcodeValidator, IPostcodeIoService postcodeIoService, ILogger<PostcodeValidator> logger)
        {
            _regexPostcodeValidator = regexPostcodeValidator;
            _postcodeIoService = postcodeIoService;
            _logger = logger;
        }

        /// <summary>
        /// Validates postcode by checking using Regex.  If this says it's valid, it checks PostcodeIO's validation endpoint.
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

            // not validating whether postcode is valid by checking in DB since it contains retired postcodes
            try
            {
                bool doesPostcodeIoThinkPostcodeIsValid = await _postcodeIoService.IsPostcodeValidAsync(postcode, CancellationToken.None);
                return doesPostcodeIoThinkPostcodeIsValid;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error calling PostcodeIO to validate postcode. Returning that postcode is valid since it passes Regex", ex);
                return true;
            }
         
        }
    }
}
