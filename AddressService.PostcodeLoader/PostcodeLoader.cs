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
        private int _numberOfInvalidRows = 0;
        private int _numberOfRows = 0;

        private readonly List<string> _invalidPostcodes = new List<string>();

        private void AddNonNullInvalidPostcode(string postcode)
        {
            if (String.IsNullOrWhiteSpace(postcode))
            {
                return;
            }

            if (!postcode.StartsWith("NPT") && !postcode.StartsWith("GIR"))
            {
                _invalidPostcodes.Add(postcode);
            }

        }

        private void Initialise()
        {
            _numberOfInvalidPostcodes = 0;
            _numberOfInvalidLatitudes = 0;
            _numberOfInvalidLongitudes = 0;
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

            Console.WriteLine($"Loading postcodes into staging table took {stopWatch.Elapsed} for {_numberOfRows} rows");

            int validRows = _numberOfRows - _numberOfInvalidRows;

            Console.WriteLine($"Total rows processed: {_numberOfRows}");
            Console.WriteLine($"Total valid rows: {validRows}");
            Console.WriteLine($"Total invalid rows: {_numberOfInvalidRows}");
            Console.WriteLine($"Invalid rows percentage: {Math.Round(GetInvalidRowsPercentage(), 4)}%");
            Console.WriteLine($"Total invalid postcodes: {_numberOfInvalidPostcodes}");
            Console.WriteLine($"Total invalid latitudes: {_numberOfInvalidLatitudes}");
            Console.WriteLine($"Total invalid longitudes: {_numberOfInvalidLongitudes}");

            Console.WriteLine();
            if (_invalidPostcodes.Any())
            {
                Console.WriteLine($"Invalid postcodes (excluding GIR and NPT)");

                foreach (string invalidPostcode in _invalidPostcodes.OrderBy(x => x))
                {
                    Console.WriteLine(invalidPostcode);
                }
            }
            else
            {
                Console.WriteLine("There were no invalid postcodes (excluding nulls and postcodes starting with NPT and GIR)");
            }
        }

        private decimal GetInvalidRowsPercentage()
        {
            decimal invalidRowsPercentage = (decimal)_numberOfInvalidRows / _numberOfRows;
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

            if (!postcodeIsValid)
            {
                AddNonNullInvalidPostcode(postcode);
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

                dataTable.Rows.Add(dataRow);
            }
        }

        private DataTable CreateDataTable()
        {

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Postcode", typeof(string));
            dataTable.Columns.Add("Latitude", typeof(decimal));
            dataTable.Columns.Add("Longitude", typeof(decimal));

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
    }

}
