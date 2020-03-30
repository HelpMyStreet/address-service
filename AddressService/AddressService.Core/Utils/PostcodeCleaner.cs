﻿namespace AddressService.Core.Utils
{
    public static class PostcodeCleaner
    {
        public static string CleanPostcode(string postcode)
        {
            postcode = postcode.Replace(" ", "").ToUpper();
            return postcode;
        }
    }
}
