using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace AddressService.Core.Utils
{
    public class AddressComparer : IComparer<string>
    {
        public AddressComparer()
        { }

        public int Compare(string a, string b)
        {
            if (a == b)
                return 0;

            string[] aSplit, bSplit;
            
            //splitting into sequences of numbers and characters
            aSplit = Regex.Split(a, "([0-9]+)");
            bSplit = Regex.Split(b, "([0-9]+)");

            //don't necessarily like the early returns but in this case it's helpful...
            for (int i = 0; i < aSplit.Length && i < bSplit.Length; i++)
            {
                //skip identical sequences
                if (aSplit[i] == bSplit[i])
                    continue;

                //if either sequences can't be parsed as an int, use a string comparrison
                if (!int.TryParse(aSplit[i], out int aInt))
                    return aSplit[i].CompareTo(bSplit[i]);

                if (!int.TryParse(bSplit[i], out int bInt))
                    return aSplit[i].CompareTo(bSplit[i]);

                //both parsed as ints, use int comparrison
                return
                    aInt.CompareTo(bInt);
            }

            //Rare scenario where one string is a perfect substring of another
            if (aSplit.Length > bSplit.Length)
                return -1;
            else if (aSplit.Length < bSplit.Length)
                return 1;
            else
                return 0;         
        }


    }
}
