using HelpMyStreet.Utils.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AddressService.Core.Validation;

namespace AddressService.PostcodeLoader
{
    public class PostcodeLoader
    {
        private readonly RegexPostcodeValidator _regexPostcodeValidator;

        public PostcodeLoader(RegexPostcodeValidator regexPostcodeValidator)
        {
            _regexPostcodeValidator = regexPostcodeValidator;
        }

        private int _numberOfInvalidPostcodes = 0;
        private int _numberOfInvalidLatitudes = 0;
        private int _numberOfInvalidLongitudes = 0;
        private int _numberOfTerminatedPostcodes = 0;
        private int _numberOfInvalidRows = 0;
        private int _numberOfRows = 0;

        private readonly List<string> _invalidPostcodes = new List<string>();

        private void Initialise()
        {
            _numberOfInvalidPostcodes = 0;
            _numberOfInvalidLatitudes = 0;
            _numberOfInvalidLongitudes = 0;
            _numberOfTerminatedPostcodes = 0;
            _numberOfInvalidRows = 0;
            _numberOfRows = 0;
        }

        public void LoadPostcodesIntoStagingTable(string postCodeFileLocation, string connectionString, int batchSize, decimal maxInvalidRowsPercentage)
        {
            Initialise();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                TruncateStagingTable(sqlConnection);
                SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

                try
                {
                    DataTable dataTable = CreateDataTable();
                    using (TextReader reader = new StreamReader(postCodeFileLocation))
                    {
                        IEnumerable<string> lines = reader.Lines().Skip(1);

                        int numberOfRowsProcessedInThisBatch = 0;
                        foreach (string row in lines)
                        {
                            _numberOfRows++;
                            AddDataRowToDataTable(dataTable, row);

                            numberOfRowsProcessedInThisBatch++;
                            if (numberOfRowsProcessedInThisBatch >= batchSize)
                            {
                                BulkInsert(sqlConnection, sqlTransaction, dataTable);
                                Console.WriteLine($"Processed {batchSize} rows");
                                numberOfRowsProcessedInThisBatch = 0;
                                dataTable.Rows.Clear();
                            }
                        }

                        // insert any remaining rows
                        BulkInsert(sqlConnection, sqlTransaction, dataTable);
                        Console.WriteLine($"Processed {numberOfRowsProcessedInThisBatch} rows");

                        if (GetInvalidRowsPercentage() <= maxInvalidRowsPercentage)
                        {
                            Console.WriteLine("Committing transaction");
                            sqlTransaction.Commit();
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("#############################################################################");
                            Console.WriteLine("## ERROR: Not committing transaction as there are too many invalid rows");
                            Console.WriteLine("#############################################################################");
                            Console.WriteLine();
                            sqlTransaction.Rollback();
                        }

                    }

                }
                catch (Exception ex)
                {
                    sqlTransaction.Rollback();
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine($"Loading postcodes into staging table took {stopWatch.Elapsed} for {_numberOfRows:N0} rows");

            int validRows = _numberOfRows - _numberOfInvalidRows;

            Console.WriteLine($"Total rows processed: {_numberOfRows:N0}");
            Console.WriteLine($"Total valid rows: {validRows:N0}");
            Console.WriteLine($"Total invalid rows: {_numberOfInvalidRows:N0}");
            Console.WriteLine($"Invalid rows percentage: {Math.Round(GetInvalidRowsPercentage(), 2)}%");
            Console.WriteLine($"Total terminated postcodes: {_numberOfTerminatedPostcodes}");
            Console.WriteLine($"Terminated postcode percentage: {Math.Round((decimal)_numberOfTerminatedPostcodes / _numberOfRows * 100, 2)}%");
            Console.WriteLine($"Total invalid postcodes: {_numberOfInvalidPostcodes:N0}");
            Console.WriteLine($"Total invalid latitudes: {_numberOfInvalidLatitudes:N0}");
            Console.WriteLine($"Total invalid longitudes: {_numberOfInvalidLongitudes:N0}");

            Console.WriteLine();
            if (_invalidPostcodes.Any())
            {
                Console.WriteLine($"Invalid Postcodes List (excludes nulls and empty postcodes):");

                foreach (string invalidPostcode in _invalidPostcodes.OrderBy(x => x))
                {
                    Console.WriteLine(invalidPostcode);
                }
            }
            else
            {
                Console.WriteLine("There were no invalid postcodes to display (excludes null and empty postcodes)");
            }
        }

        private decimal GetInvalidRowsPercentage()
        {
            decimal invalidRowsPercentage = (decimal)_numberOfInvalidRows / _numberOfRows * 100;
            return invalidRowsPercentage;
        }


        private void TruncateStagingTable(SqlConnection sqlConnection)
        {
            using (SqlCommand sqlCmd = new SqlCommand("TRUNCATE TABLE [Staging].[Postcode_Staging]", sqlConnection))
            {
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.ExecuteNonQuery();
            }
        }

        private void BulkInsert(SqlConnection sqlConnection, SqlTransaction transaction, DataTable dataTable)
        {

            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.TableLock, transaction))
            {
                sqlBulkCopy.ColumnMappings.Add("Postcode", "Postcode");
                sqlBulkCopy.ColumnMappings.Add("Latitude", "Latitude");
                sqlBulkCopy.ColumnMappings.Add("Longitude", "Longitude");
                sqlBulkCopy.ColumnMappings.Add("IsActive", "IsActive");
                sqlBulkCopy.DestinationTableName = "[Staging].[Postcode_Staging]";
                sqlBulkCopy.BulkCopyTimeout = 0;
                sqlBulkCopy.WriteToServer(dataTable);
            }
        }

        private void AddDataRowToDataTable(DataTable dataTable, string input)
        {
            string[] split = input.Split(',');

            bool rowHasEnoughColumns = split.Length > 43;
            if (!rowHasEnoughColumns)
            {
                Console.WriteLine("CS row doesn't have enough columns");
                return;
            }

            string postcode = split[0];

            if (postcode != null)
            {
                postcode = postcode.Replace("\"", "");
                postcode = PostcodeFormatter.FormatPostcode(postcode);
            }

            bool postcodeIsValid = _regexPostcodeValidator.IsPostcodeValid(postcode);

            bool latitudeIsValid = decimal.TryParse(split[42], out decimal latitude);
            if (latitudeIsValid)
            {
                latitudeIsValid = latitude >= -90 && latitude <= 90;
            }

            bool longitudeIsValid = decimal.TryParse(split[43], out decimal longitude);
            if (longitudeIsValid)
            {
                longitudeIsValid = longitude >= -180 && longitude <= 180;
            }


            var introductionDateString = split[3];
            var terminationDateString = split[4];

            var isPostCodeActive = OnsActivePostcodeDeterminer.IsPostcodeActive(introductionDateString, terminationDateString, DateTime.UtcNow);


            if (!postcodeIsValid)
            {
                _invalidPostcodes.Add(postcode);
                _numberOfInvalidPostcodes++;

            }

            if (!latitudeIsValid)
            {
                _numberOfInvalidLatitudes++;
            }

            if (!longitudeIsValid)
            {
                _numberOfInvalidLongitudes++;
            }

            if (!isPostCodeActive)
            {
                _numberOfTerminatedPostcodes++;
            }

            if (!postcodeIsValid || !latitudeIsValid || !longitudeIsValid)
            {
                _numberOfInvalidRows++;
            }
            else
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow["Postcode"] = postcode;
                dataRow["Latitude"] = latitude;
                dataRow["Longitude"] = longitude;
                dataRow["IsActive"] = isPostCodeActive;

                dataTable.Rows.Add(dataRow);
            }
        }

        private DataTable CreateDataTable()
        {

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Postcode", typeof(string));
            dataTable.Columns.Add("Latitude", typeof(decimal));
            dataTable.Columns.Add("Longitude", typeof(decimal));
            dataTable.Columns.Add("IsActive", typeof(bool));

            return dataTable;
        }


        public void LoadFromStagingTableAndSwitch(string connectionString)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCmd = new SqlCommand("EXEC [Staging].[LoadFromStagingTableAndSwitch]", sqlConnection))
                {
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandTimeout = 0; // the merge will take a while (takes around 15 minutes on my laptop)
                    sqlCmd.ExecuteNonQuery();
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Loading, updating and switching took {stopwatch.Elapsed}");
        }

        public void TruncateSwitchTable(string connectionString)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCmd = new SqlCommand("TRUNCATE TABLE [Staging].[Postcode_Switch]", sqlConnection))
                {
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandTimeout = 30;
                    sqlCmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine($"Switch table ([Staging].[Postcode_Switch]) truncation complete");
        }

        public void TruncateStagingTable(string connectionString)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCmd = new SqlCommand("TRUNCATE TABLE [Staging].[Postcode_Staging]", sqlConnection))
                {
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandTimeout = 30;
                    sqlCmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine($"Staging table ([Staging].[Postcode_Staging]) truncation complete");
        }

        public void TruncatePreComputedNearestPostcodes(string connectionString)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCmd = new SqlCommand("TRUNCATE TABLE [Address].[PreComputedNearestPostcodes]", sqlConnection))
                {
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandTimeout = 30;
                    sqlCmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine($"Precomputed nearby postcodes truncation complete ([Address].[PreComputedNearestPostcodes])");
        }
    }

}
