using HelpMyStreet.Utils.Utils;
using System;
using System.Text.RegularExpressions;

namespace AddressService.Core.Validation
{
    public class RegexPostcodeValidator : IRegexPostcodeValidator
    {
        // Excludes Girobank/Santander's GIR 0AA and Newport's NPT postcodes that were retired in 1984
        private static readonly Regex _postCodeRegex = new Regex("^((([A-Z]|[a-z]){1,2})|NPT|GIR)[0-9][0-9A-Z]?\\s?[0-9]([A-Z]|[a-z])([A-Z]|[a-z])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Validates postcode using Regex and aims to produce no false negatives.
        /// </summary>
        /// <param name="postcode">Postcode to validate</param>\
        /// <returns>Whether postcode is valid using regex</returns>
        public bool IsPostcodeValid(string postcode)
        {
            if (String.IsNullOrWhiteSpace(postcode))
            {
                return false;
            }

            postcode = PostcodeFormatter.FormatPostcode(postcode);

            if (!_postCodeRegex.IsMatch(postcode))
            {
                return false;
            }

            return true;
        }
    }
}
