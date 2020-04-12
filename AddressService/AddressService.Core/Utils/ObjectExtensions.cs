using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace AddressService.Core.Utils
{
    public static class ObjectExtensions
    {
        public static bool IsValid(this object o, out ICollection<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(o, new ValidationContext(o, null, null), validationResults, true);
        }

        public static (string Name, int Count) RemoveNumbersAndSelectMostCommon(this IEnumerable<string> str)
        {
            //regex to pull out a sequence of numbers followed by an optional single letter and then a space
            //OR pull out a sequence of numbers followed by a letter followed by a sequence of numbers followed by a space.
            //that last one is because Scotland *loves* including "1/2 Street Name"
            var mostFrequent = str.Select(x => Regex.Replace(x, @"([\d-]+[\S]?[\s])|([\d-]+[\S][\d-]+[\S]?[\s])", ""))
                .GroupBy(y => y)
                .Select(z => new
                {
                    Name = z.Key,
                    Count = z.Count()
                })
                .OrderByDescending(a => a.Count)
                .First();

            return (mostFrequent.Name, mostFrequent.Count);
        }

        public static IEnumerable<int> ExtractNumbers(this IEnumerable<string> str, string matchString)
        {
            //Might need to get a bit more clever and handle sequences like "1/2" but this is probably fine)
            return str.Where(x => x.Contains(matchString)
                    && x.Any(char.IsDigit))
                .Select(y => Regex.Match(y, @"[\d-]+").Value)
                //the below seems to be the safest way of attempting to parse the regex matches as an int
                .Select(z => int.TryParse(z, out int n) ? n : (int?)null)
                .Where(n => n.HasValue)
                .Select(n => n.Value);
        }

        //attempts to describe building numbers in a human friendly way
        public static string GenerateNumbersDescriptor(this IEnumerable<int> numbers, int rangeTolerance)
        {
            if (numbers == null || !numbers.Any())
                return "";

            int Min = numbers.Min();
            int Max = numbers.Max();

            //filters out postcodes with single addresses etc
            if (Max - Min < rangeTolerance)
                return "";

            //could probably do this in one, but I'm not that smart
            bool AllEven = numbers.All(x => x % 2 == 0);
            bool AllOdd = numbers.All(x => x % 2 == 1);

            string result;
            if (AllEven)
                result = "Even numbers, ";
            else if (AllOdd)
                result = "Odd numbers, ";
            else
                result = "";

            //assume if the street numbers start at 1 or 2 it's a whole street postcode
            //short of scanning nearby postcodes I don't think there's any other way to know
            //if not assume partial postcode and provide a range
            if (Min > 2)
            {
                result = result + $"{Min}-{Max}, ";
            }

            return result;
        }
    }
}
