using System;

namespace AddressService.Core.Utils
{
    public static class PostcodeCleaner
    {
        public static string CleanPostcode(string postcode)
        {
            // this method is not responsible for validation, but equally we don't want it to error
            if (!String.IsNullOrWhiteSpace(postcode))
            {
                postcode = postcode.Replace(" ", "").Trim().ToUpper();

                // don't try to format postcode that is obviously invalid
                if (postcode.Length >= 5 && postcode.Length <= 7)
                {
                    postcode = postcode.Replace(" ", "").Trim().ToUpper();
                    postcode = postcode.Insert(postcode.Length - 3, " ");
                }
            }

            return postcode;
        }
    }
}
