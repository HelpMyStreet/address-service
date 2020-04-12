using AddressService.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AddressService.Core.Utils
{
    public class FriendlyNameGenerator : IFriendlyNameGenerator
    {
        private const double ThresholdForMost = 0.5;
        /// <summary>
        /// Adds a friendly name to PostcodeDtos based on address details within the postcode
        /// This rapidly became a mess, I'm sorry :(
        /// </summary>
        /// <param name="postcodeDtos"></param>
        public void GenerateFriendlyName(PostcodeDto postcodeDto)
        {
            IEnumerable<AddressDetailsDto> details = postcodeDto.AddressDetails.Where(x => !string.IsNullOrWhiteSpace(x.AddressLine1));
            if (details == null || !details.Any())
            {
                postcodeDto.FriendlyName = "";
                return; //may want a different default but seems better to resort to postcode only in unhandleable cases
            }

            int totalAddresses = details.Count();
            //Identify and return simple addresses
            double simplePostcodeFraction = (double)details.Where(x => string.IsNullOrWhiteSpace(x.AddressLine2)).Count() / totalAddresses;

                            //Simple Address Block
                if (simplePostcodeFraction >= ThresholdForMost)
                {
                    //From a test sample seems to happen fairly frequently
                    //filtering down to only only entries in the "dominant" case
                    // this should clear out the occasional building name / business in a residential postcode
                    IEnumerable<string> firstLine = details.Where(x => !string.IsNullOrWhiteSpace(x.AddressLine1))
                        .Select(x => x.AddressLine1);

                    (string name, int count) = firstLine.RemoveNumbersAndSelectMostCommon();

                    if ((double)count / totalAddresses >= ThresholdForMost)
                    {
                        string numberRange = firstLine.ExtractNumbers(name)
                            .GenerateNumbersDescriptor(5);
                        postcodeDto.FriendlyName = numberRange + name;
                        return;

                    }
                }

                //simple flat block
                //doesn't seem to be another way to identify a block of flats easily
                double simpleFlatFraction = (double)details.Where(x =>
                        x.AddressLine1.Contains("Flat")
                        || x.AddressLine1.Contains("Apartment")
                        || x.AddressLine1.All(char.IsDigit)
                        ).Count()
                    / totalAddresses;

                if (simpleFlatFraction >= ThresholdForMost)
                {
                    string result = details.Where(x => !string.IsNullOrWhiteSpace(x.AddressLine2))
                        .Select(y => y.AddressLine2)
                        .GroupBy(z => z)
                        .OrderByDescending(a => a.Count())
                        .First()
                        .Key;

                    // see if we can add a street name too
                    double hasThirdLineFraction = (double)details.Where(x => !string.IsNullOrWhiteSpace(x.AddressLine3))
                        .Count()
                        / totalAddresses;

                    if (hasThirdLineFraction > ThresholdForMost)
                    {
                        (string nameLine3, int countLine3) = details.Where(x => !string.IsNullOrWhiteSpace(x.AddressLine3))
                            .Select(y => y.AddressLine3)
                            .RemoveNumbersAndSelectMostCommon();

                        if ((double)countLine3 / totalAddresses > ThresholdForMost)
                            result = string.Concat(result, ", ", nameLine3);
                    }

                    postcodeDto.FriendlyName = result;
                    return;
                }

                //case is neither a simple flat nor a simple street address - now it gets more difficult
                //most likely scenario is "# street name, district"
                //Hence move through each address line at a time, stripping out numbers and checking if the count meets the tolerance criteria
                //hopefully all of the flats have been caught by this out, and non-residential addresses should only match on street name
                IEnumerable<string> addressLine1 = details.Where(x => !string.IsNullOrWhiteSpace(x.AddressLine1))
                    .Select(y => y.AddressLine1);

                (string line1Name, int line1Count) = addressLine1.RemoveNumbersAndSelectMostCommon();

                if ((double)line1Count / totalAddresses > ThresholdForMost)
                {
                    string numberRange = addressLine1.ExtractNumbers(line1Name)
                        .GenerateNumbersDescriptor(5);
                    //probably of the farm "# street name, region, locality"
                    postcodeDto.FriendlyName = numberRange + line1Name;
                    return;
                }

                IEnumerable<string> addressLine2 = details.Where(x => !string.IsNullOrWhiteSpace(x.AddressLine2))
                    .Select(y => y.AddressLine2);

                (string line2Name, int line2Count) = addressLine2.RemoveNumbersAndSelectMostCommon();

                if ((double)line2Count / totalAddresses > ThresholdForMost)
                {
                    string numberRange = addressLine2.ExtractNumbers(line2Name)
                        .GenerateNumbersDescriptor(5);
                    //probably non residential addresses of the form "business name, # street name"
                    postcodeDto.FriendlyName = numberRange + line2Name;
                    return;
                }

                IEnumerable<string> addressLine3 = details.Where(x => !string.IsNullOrWhiteSpace(x.AddressLine3))
                    .Select(y => y.AddressLine3);

                (string line3Name, int line3Count) = addressLine3.RemoveNumbersAndSelectMostCommon();
                if ((double)line3Count / totalAddresses > ThresholdForMost)
                {
                    string numberRange = addressLine3.ExtractNumbers(line3Name)
                        .GenerateNumbersDescriptor(5);
                    //weird edge cases with building names and buisness names on line 1 and 2
                    //should pull streets out of line 3
                    postcodeDto.FriendlyName = numberRange + line3Name;
                    return;
                }

                //Everything has failed :(
                postcodeDto.FriendlyName = "";
            
            return;
        }



    }
}
