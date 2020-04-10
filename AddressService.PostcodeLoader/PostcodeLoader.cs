using HelpMyStreet.Utils.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AddressService.PostcodeLoader
{
    public class PostcodeLoader
    {
        private int _invalidPostcodes = 0;
        private int _invalidLatitudes = 0;
        private int _invalidLongitudes = 0;
        private int _invalidRows = 0;
        private int _totalRows = 0;

        private void Initialise()
        {
            _invalidPostcodes = 0;
            _invalidLatitudes = 0;
            _invalidLongitudes = 0;
            _invalidRows = 0;
            _totalRows = 0;
        }

        public void LoadPostcodes(string postCodeFileLocation, string connectionString, int batchSize, decimal maxInvalidRowsPercentage)
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
                            _totalRows++;
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

                        if (GetInvalidRowsPercentage() <= maxInvalidRowsPercentage)
                        {

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

            Console.WriteLine($"Loading postcodes into staging table took {stopWatch.Elapsed} for {_totalRows} rows");

            int validRows = _totalRows - _invalidRows;

            Console.WriteLine($"Total rows processed: {_totalRows}");
            Console.WriteLine($"Total valid rows: {validRows}");
            Console.WriteLine($"Total invalid rows: {_invalidRows}");
            Console.WriteLine($"Invalid rows percentage: {Math.Round(GetInvalidRowsPercentage(), 4)}%");
            Console.WriteLine($"Total invalid postcodes: {_invalidPostcodes}");
            Console.WriteLine($"Total invalid latitudes: {_invalidLatitudes}");
            Console.WriteLine($"Total invalid longitudes: {_invalidLongitudes}");

        }

        private decimal GetInvalidRowsPercentage()
        {
            decimal invalidRowsPercentage = (decimal)_invalidRows / _totalRows;
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
            bool postCodeIsNull = String.IsNullOrWhiteSpace(postcode);

            if (postcode != null)
            {
                postcode = postcode.Replace("\"", "");
                postcode = PostcodeFormatter.FormatPostcode(postcode);
            }

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

            if (postCodeIsNull)
            {
                _invalidPostcodes++;
            }

            if (!latitudeIsValid)
            {
                _invalidLatitudes++;
            }

            if (!longitudeIsValid)
            {
                _invalidLongitudes++;
            }

            if (postCodeIsNull || !latitudeIsValid || !longitudeIsValid)
            {
                _invalidRows++;
            }

            DataRow dataRow = dataTable.NewRow();
            dataRow["Postcode"] = postcode;
            dataRow["Latitude"] = latitude;
            dataRow["Longitude"] = longitude;

            dataTable.Rows.Add(dataRow);
        }

        private DataTable CreateDataTable()
        {

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Postcode", typeof(string));
            dataTable.Columns.Add("Latitude", typeof(decimal));
            dataTable.Columns.Add("Longitude", typeof(decimal));

            return dataTable;
        }

    }

}
