using System;
using System.Globalization;

namespace AddressService.PostcodeLoader
{
    public class OnsActivePostcodeDeterminer
    {
        public static bool IsPostcodeActive(string dateOfIntrodutionString, string dateOfTerminationString, DateTime dateNow)
        {
            if (dateOfTerminationString == null)
            {
                return true;
            }

            dateOfTerminationString = dateOfTerminationString.Replace("\"", "");

            if (dateOfIntrodutionString != null)
            {
                dateOfIntrodutionString = dateOfIntrodutionString.Replace("\"", "");
            }

            bool isDateOfIntrodutionValid = TryParseDate(dateOfIntrodutionString, out DateTime? dateOfIntrodution);
            bool isDateOfTerminationValid = TryParseDate(dateOfTerminationString, out DateTime? dateOfTermination);

            // this means the date of termination is null, in which case the postcode is active, or the date is invalid, in which case we'll assume the postcode is active
            if (!isDateOfTerminationValid)
            {
                return true;
            }

            // the introduction date should always be valid, but if it isn't we'll assume the postcode is active
            if (!isDateOfIntrodutionValid)
            {
                return true;
            }

            // introduction date is after the termination date, which means the postcode has been reactivated
            if (dateOfIntrodution.Value >= dateOfTermination.Value)
            {
                return true;
            }

            return false;
        }

        private static bool TryParseDate(string yearMonth, out DateTime? dateTime)
        {

            dateTime = null;
            if (String.IsNullOrWhiteSpace(yearMonth))
            {
                return false;
            }

            if (yearMonth.Length != 6)
            {
                return false;
            }

            bool isDateValid = DateTime.TryParseExact(yearMonth, "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime date);
            dateTime = date;

            return isDateValid;

        }
    }
}
