namespace AddressService.Core.Validation
{
    public interface IRegexPostcodeValidator
    {
        /// <summary>
        /// Validates postcode using Regex
        /// </summary>
        /// <param name="postcode">Postcode to validate</param>
        /// <returns>Whether postcode is valid using regex</returns>
        bool IsPostcodeValid(string postcode);
    }
}