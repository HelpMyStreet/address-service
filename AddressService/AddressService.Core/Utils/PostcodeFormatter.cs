using System;
using System.Linq;

namespace AddressService.Core.Utils
{
    public static class PostcodeFormatter
    {
        /// <summary>
        /// Returns a new postcode string in a consistent format
        /// </summary>
        /// <param name="postcode">Postcode to format</param>
        /// <returns>New formatted postcode string</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when postcode is null</exception>
        public static string FormatPostcode(string postcode)
        {
            if (postcode == null)
            {
                throw new ArgumentNullException(nameof(postcode));
            }

            string cleanedPostcode = String.Copy(postcode);

            // this method is not responsible for validation, but equally we don't want it to error
            if (!IsEmptyOrWhiteSpace(cleanedPostcode))
            {
                cleanedPostcode = cleanedPostcode.Replace(" ", "").ToUpper();

                // don't try to format a postcode that is obviously invalid
                if (cleanedPostcode.Length >= 5 && cleanedPostcode.Length <= 7)
                {
                    cleanedPostcode = cleanedPostcode.Insert(cleanedPostcode.Length - 3, " ");
                }
            }

            return cleanedPostcode;
        }

        private static bool IsEmptyOrWhiteSpace(string value) => value.All(char.IsWhiteSpace);
    }
}
