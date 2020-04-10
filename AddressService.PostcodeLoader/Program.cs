using System;

namespace AddressService.PostcodeLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            var postcodeLoader = new PostcodeLoader();
            postcodeLoader.LoadPostcodes();

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }
}
