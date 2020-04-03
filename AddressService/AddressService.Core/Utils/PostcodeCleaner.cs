using System;

namespace AddressService.Core.Utils
{
    public static class PostcodeCleaner
    {
        /// <summary>
        /// Put postcodes into a consistent format
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public static string CleanPostcode(string postcode)
        {
            // this method is not responsible for validation, but equally we don't want it to error
            if (!String.IsNullOrWhiteSpace(postcode))
            {
                postcode = postcode.Replace(" ", "").ToUpper();

                // don't try to format a postcode that is obviously invalid
                if (postcode.Length >= 5 && postcode.Length <= 7)
                {
                    postcode = postcode.Insert(postcode.Length - 3, " ");
                }
            }

            return postcode;
        }
    }
}
