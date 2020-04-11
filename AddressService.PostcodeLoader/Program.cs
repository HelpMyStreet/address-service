using System;
using AddressService.Core.Validation;
using Microsoft.Extensions.Configuration;

namespace AddressService.PostcodeLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", true)
                .Build();


            string connectionString = config.GetSection("ConnectionStrings:AddressService").Value;

            if (String.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("Connection string is null");
            }

            string postCodeFileLocation = config.GetSection("Settings:postCodeFileLocation").Value;
            if (String.IsNullOrWhiteSpace(postCodeFileLocation))
            {
                throw new Exception("Postcode file is null");
            }

            if (!Int32.TryParse(config.GetSection("Settings:batchSize").Value, out int batchSize))
            {
                throw new Exception("Can't parse batchSize setting");
            }

            if (batchSize < 1)
            {
                throw new Exception("Batch size can't be less than 1");
            }

            if (!Decimal.TryParse(config.GetSection("Settings:maxInvalidRowsPercentage").Value, out decimal maxInvalidRowsPercentage))
            {
                throw new Exception("Can't parse maxInvalidRowsPercentage setting");
            }

            var postcodeLoader = new PostcodeLoader(new RegexPostcodeValidator());
            postcodeLoader.LoadPostcodes(postCodeFileLocation, connectionString, batchSize, maxInvalidRowsPercentage);

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }
}
