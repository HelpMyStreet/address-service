using AddressService.Core.Config;
using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AddressService.Repo
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IOptionsSnapshot<ConnectionStrings> _connectionStrings;

        public Repository(ApplicationDbContext context, IMapper mapper, IOptionsSnapshot<ConnectionStrings> connectionStrings)
        {
            _context = context;
            _mapper = mapper;
            _connectionStrings = connectionStrings;
        }

        public async Task<IEnumerable<PostcodeDto>> GetPostcodesAsync(IEnumerable<string> postcodes)
        {
            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.AddressService))
            {
                DataTable postcodesDataTable = CreatePostcodeOnlyDataTable(postcodes);

                Dictionary<int, PostcodeDto> postcodeDictionary = new Dictionary<int, PostcodeDto>();

                IEnumerable<PostcodeDto> result = await connection.QueryAsync<PostcodeDto, AddressDetailsDto, PostcodeDto>("[Address].[GetPostcodesAndAddresses]",
                    commandType: CommandType.StoredProcedure,
                    map: (p, ad) =>
                    {
                        if (!postcodeDictionary.TryGetValue(p.Id, out PostcodeDto postcode))
                        {
                            postcode = p;
                            postcodeDictionary.Add(postcode.Id, postcode);
                        }

                        postcode.AddressDetails.Add(ad);

                        return p;
                    },
                    splitOn: "PostcodeId",
                    param: new {PostCodes = postcodesDataTable},
                    commandTimeout: 30);

                return postcodeDictionary.Values;
            }
        }

        private DataTable CreatePostcodeOnlyDataTable(IEnumerable<string> postcodes)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Postcode", typeof(string));
            foreach (string postcode in postcodes)
            {
                DataRow row = dataTable.NewRow();
                row["Postcode"] = postcode;
                dataTable.Rows.Add(row);
            }

            dataTable.EndLoadData();

            return dataTable;
        }

        public async Task SaveAddressesAsync(IEnumerable<PostcodeDto> postcodes)
        {
            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.AddressService))
            {
                DataTable addressDataTable = CreateAddressDataTable(postcodes);

                await connection.ExecuteAsync("[Address].[SaveAddresses]",
                    commandType: CommandType.StoredProcedure,
                    param: new {AddressDetails = addressDataTable},
                    commandTimeout: 30);
            }
        }

        private DataTable CreateAddressDataTable(IEnumerable<PostcodeDto> postcodes)
        {

            DataTable addresssDataTable = new DataTable();
            addresssDataTable.Columns.Add("AddressLine1", typeof(string));
            addresssDataTable.Columns.Add("AddressLine2", typeof(string));
            addresssDataTable.Columns.Add("AddressLine3", typeof(string));
            addresssDataTable.Columns.Add("Locality", typeof(string));
            addresssDataTable.Columns.Add("Postcode", typeof(string));
            addresssDataTable.Columns.Add("LastUpdated", typeof(DateTime));

            foreach (PostcodeDto postcode in postcodes)
            {
                foreach (AddressDetailsDto addressDetail in postcode.AddressDetails)
                {
                    DataRow addressRow = addresssDataTable.NewRow();
                    addressRow["AddressLine1"] = addressDetail.AddressLine1;
                    addressRow["AddressLine2"] = addressDetail.AddressLine2;
                    addressRow["AddressLine3"] = addressDetail.AddressLine3;
                    addressRow["Locality"] = addressDetail.Locality;
                    addressRow["Postcode"] = postcode.Postcode;
                    addressRow["LastUpdated"] = postcode.LastUpdated;

                    addresssDataTable.Rows.Add(addressRow);
                }
            }

            return addresssDataTable;
        }


        public async Task<bool> IsPostcodeInDb(string postcode)
        {
            bool result = await _context.PostCode.AnyAsync(x => x.Postcode == postcode);
            return result;
        }


        //public async Task<IEnumerable<NearestPostcodeDto>> GetNearestPostcodesAsync(string postcode, int distanceInMetres, int maxNumberOfResults)
        //{
        //    using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.AddressService))
        //    {

        //        var sw = new Stopwatch();
        //        sw.Start();
        //        IEnumerable<NearestPostcodeDto> result = await connection.QueryAsync<NearestPostcodeDto>("[Address].[GetNearestPostcodes]",
        //            commandType: CommandType.StoredProcedure,
        //            param: new { Postcode = postcode, DistanceInMetres = distanceInMetres, MaxNumberOfResults = maxNumberOfResults },
        //            commandTimeout: 15);
        //        sw.Stop();
        //        Debug.WriteLine($"GetNearestPostcodesAsync Dapper: {sw.ElapsedMilliseconds}");

        //        return result;
        //    }
        //}

        public async Task<IEnumerable<NearestPostcodeDto>> GetNearestPostcodesAsync(string postcode, double distanceInMetres, int maxNumberOfResults)
        {
            //var sw = new Stopwatch();
            //sw.Start();
            //var nearestPostcodeDtos = new List<NearestPostcodeDto>();
            //using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.Value.AddressService))
            //{
            //    await sqlConnection.OpenAsync();

            //    using (SqlCommand sqlCommand = new SqlCommand("[Address].[GetNearestPostcodes]", sqlConnection))
            //    {
            //        sqlCommand.CommandType = CommandType.StoredProcedure;
            //        sqlCommand.Parameters.Add(new SqlParameter("@Postcode", postcode));
            //        sqlCommand.Parameters.Add(new SqlParameter("@DistanceInMetres", distanceInMetres));
            //        sqlCommand.Parameters.Add(new SqlParameter("@MaxNumberOfResults", maxNumberOfResults));

            //        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
            //        {
            //            while (await sqlDataReader.ReadAsync())
            //            {
            //                var nearestPostcodeDto = new NearestPostcodeDto();
            //                nearestPostcodeDto.Postcode = sqlDataReader.GetFieldValue<string>(0);
            //                nearestPostcodeDto.DistanceInMetres = (int) sqlDataReader.GetFieldValue<double>(1);

            //                nearestPostcodeDtos.Add(nearestPostcodeDto);
            //            }

            //            sw.Stop();
            //            Debug.WriteLine($"GetNearestPostcodesAsync ADO SQlDataReader: {sw.ElapsedMilliseconds}");
            //            return nearestPostcodeDtos;
            //        }

            //    }
            //}

            //////////////////////

            //var sw = new Stopwatch();
            //sw.Start();

            //var sw2 = new Stopwatch();
            //sw2.Start();

            //var nearestPostcodeDtos = new List<NearestPostcodeDto>();
            //using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.Value.AddressService))
            //{
            //    await sqlConnection.OpenAsync();

            //    var ds = new DataSet();

            //    using (SqlDataAdapter da = new SqlDataAdapter("[Address].[GetNearestPostcodes]", sqlConnection))
            //    {
            //        da.SelectCommand.CommandType = CommandType.StoredProcedure;
            //        da.SelectCommand.Parameters.Add(new SqlParameter("@Postcode", postcode));
            //        da.SelectCommand.Parameters.Add(new SqlParameter("@DistanceInMetres", distanceInMetres));
            //        da.SelectCommand.Parameters.Add(new SqlParameter("@MaxNumberOfResults", maxNumberOfResults));
            //        da.Fill(ds);
            //    }
            //    sw2.Stop();
            //    Debug.WriteLine($"GetNearestPostcodesAsync ADO Fill: {sw2.ElapsedMilliseconds}");

            //    sw2.Reset();
            //    foreach (DataRow row in ds.Tables[0].Rows)
            //    {
            //        foreach (DataColumn column in ds.Tables[0].Columns)
            //        {
            //            var nearestPostcodeDto = new NearestPostcodeDto();
            //            nearestPostcodeDto.Postcode = row["Postcode"].ToString();
            //            nearestPostcodeDto.DistanceInMetres = (int)(double.Parse(row["DistanceInMetres"].ToString()));

            //            nearestPostcodeDtos.Add(nearestPostcodeDto);
            //        }
            //    }
            //    sw2.Stop();
            //    Debug.WriteLine($"GetNearestPostcodesAsync ADO Mapping: {sw2.ElapsedMilliseconds}");

            //    sw.Stop();
            //    Debug.WriteLine($"GetNearestPostcodesAsync ADO DataSet Total: {sw.ElapsedMilliseconds}");
            //    return nearestPostcodeDtos;



            //////////////////////



            var sw = new Stopwatch();
            sw.Start();
            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.AddressService))
            {
                IEnumerable<NearestPostcodeDto> result = await connection.QueryAsync<NearestPostcodeDto>("[Address].[GetNearestPostcodes]",
                    commandType: CommandType.StoredProcedure,
                    param: new {Postcode = postcode, DistanceInMetres = distanceInMetres, MaxNumberOfResults = maxNumberOfResults},
                    commandTimeout: 15);
                sw.Stop();
                Debug.WriteLine($"GetNearestPostcodesAsync Dapper: {sw.ElapsedMilliseconds}");

                return result;

            }
        }
    }
}

