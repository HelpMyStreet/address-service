using AddressService.Core.Dto;
using AddressService.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using HelpMyStreet.Utils.Utils;

namespace AddressService.Mappers
{
    public class QasMapper : IQasMapper
    {
        public PostcodeDto MapToPostcodeDto(string postcode, IEnumerable<QasFormatRootResponse> qasFormatRootResponses)
        {
            DateTime timeNow = DateTime.UtcNow;
            postcode = PostcodeFormatter.FormatPostcode(postcode);
            PostcodeDto postcodeDto = new PostcodeDto();
            postcodeDto.Postcode = postcode;
            postcodeDto.LastUpdated = timeNow;
            
            foreach (var qasFormatRootResponse in qasFormatRootResponses)
            {
                AddressDetailsDto addressDetailsDto = new AddressDetailsDto(); 

                foreach (QasFormatAddressReponse address in qasFormatRootResponse.Address) // to deal with strange way results are returned ...
                {
                    if (!String.IsNullOrWhiteSpace(address.AddressLine1))
                    {
                        addressDetailsDto.AddressLine1 = address.AddressLine1;
                    }
                    else if(!String.IsNullOrWhiteSpace(address.AddressLine2))
                    {
                        addressDetailsDto.AddressLine2 = address.AddressLine2;
                    }
                    else if (!String.IsNullOrWhiteSpace(address.AddressLine3))
                    {
                        addressDetailsDto.AddressLine3 = address.AddressLine3;
                    }
                    else if (!String.IsNullOrWhiteSpace(address.Locality))
                    {
                        addressDetailsDto.Locality = address.Locality;
                    }
                    else if (!String.IsNullOrWhiteSpace(address.PostalCode))
                    {
                        addressDetailsDto.Postcode = PostcodeFormatter.FormatPostcode(address.PostalCode);
                    }
                }
                // filter out postcodes that weren't returned or don't have the expected postcode
                if (!String.IsNullOrWhiteSpace(addressDetailsDto.Postcode) && addressDetailsDto.Postcode == postcode)
                {
                    addressDetailsDto.LastUpdated = timeNow;
                    postcodeDto.AddressDetails.Add(addressDetailsDto);
                }
            }

            return postcodeDto;
        }

        /// <summary>
        /// Returns Format IDs grouped by postcode
        /// </summary>
        /// <param name="qasSearchRootResponses"></param>
        /// <returns></returns>
        public ILookup<string, string> GetFormatIds(IEnumerable<QasSearchRootResponse> qasSearchRootResponses)
        {
            List<Tuple<string, string>> formatIds = new List<Tuple<string, string>>();

            foreach (var qasSearchRootResponse in qasSearchRootResponses)
            {
                foreach (var result in qasSearchRootResponse.Results)
                {
                    // filter out any addresses missing a Format ID
                    if (!String.IsNullOrWhiteSpace(result.Format))
                    {
                        Tuple<string, string> postCodeWithFormatId = new Tuple<string, string>(qasSearchRootResponse.Postcode, GetFormatIdFromUri(result.Format));
                        formatIds.Add(postCodeWithFormatId);
                    }
                }
            }

            ILookup<string, string> formatIdsLookup = formatIds.ToLookup(x => x.Item1, x => x.Item2);

            return formatIdsLookup;
        }

        private string GetFormatIdFromUri(string uri)
        {
            NameValueCollection queryParameters = HttpUtility.ParseQueryString(uri);
            string id = queryParameters.Get("id");
            return id;
        }
    }
}
