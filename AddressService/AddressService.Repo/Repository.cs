﻿using AddressService.Core.Config;
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
using System.Linq;
using System.Threading.Tasks;
using AddressService.Repo.EntityFramework.Entities;

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
                    param: new { PostCodes = postcodesDataTable },
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

        public async Task SaveAddressesAndFriendlyNameAsync(IEnumerable<PostcodeDto> postcodes)
        {
            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.AddressService))
            {
                await connection.OpenAsync();
                SqlTransaction sqlTransaction = connection.BeginTransaction();
                try
                {
                    DataTable addressDataTable = CreateAddressDataTable(postcodes);
                    DataTable friendlyNameDataTable = CreateFriendlyNameDataTable(postcodes);

                    await connection.ExecuteAsync("[Address].[SaveAddresses]",
                        commandType: CommandType.StoredProcedure,
                        param: new { AddressDetails = addressDataTable },
                        commandTimeout: 30,
                        transaction: sqlTransaction);

                    await connection.ExecuteAsync("[Address].[SaveFriendlyNames]",
                        commandType: CommandType.StoredProcedure,
                        param: new { PostcodeFriendlyNames = friendlyNameDataTable },
                        commandTimeout: 30,
                        transaction: sqlTransaction);

                    sqlTransaction.Commit();
                }
                catch (Exception ex)
                {
                    sqlTransaction.Rollback();
                    throw;
                }
            }
        }

        private DataTable CreateFriendlyNameDataTable(IEnumerable<PostcodeDto> postcodes)
        {
            DataTable friendlyNameDataTable = new DataTable();
            friendlyNameDataTable.Columns.Add("Postcode", typeof(string));
            friendlyNameDataTable.Columns.Add("FriendlyName", typeof(string));

            foreach (PostcodeDto postcode in postcodes)
            {
                DataRow friendlyNameRow = friendlyNameDataTable.NewRow();
                friendlyNameRow["Postcode"] = postcode.Postcode;
                friendlyNameRow["FriendlyName"] = postcode.FriendlyName;

                friendlyNameDataTable.Rows.Add(friendlyNameRow);
            }

            return friendlyNameDataTable;
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


        public async Task<bool> IsPostcodeInDbAndActive(string postcode)
        {
            bool result = await _context.Postcode.AnyAsync(x => x.Postcode == postcode && x.IsActive);
            return result;
        }

        public async Task<IEnumerable<NearestPostcodeDto>> GetNearestPostcodesAsync(string postcode, double distanceInMetres)
        {
            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.AddressService))
            {
                IEnumerable<NearestPostcodeDto> result = await connection.QueryAsync<NearestPostcodeDto>("[Address].[GetNearestPostcodes]",
                    commandType: CommandType.StoredProcedure,
                    param: new { Postcode = postcode, DistanceInMetres = distanceInMetres },
                    commandTimeout: 15);

                return result;
            }
        }


        public async Task SavePreComputedNearestPostcodes(PreComputedNearestPostcodesDto preComputedNearestPostcodesDto)
        {
            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.AddressService))
            {
                await connection.ExecuteAsync("[Address].[SavePreComputedNearestPostcodes]",
                   commandType: CommandType.StoredProcedure,
                   param: new { Postcode = preComputedNearestPostcodesDto.Postcode, CompressedNearestPostcodes = preComputedNearestPostcodesDto.CompressedNearestPostcodes },
                   commandTimeout: 15);
            }
        }

        public async Task<PreComputedNearestPostcodesDto> GetPreComputedNearestPostcodes(string postcode)
        {
            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.AddressService))
            {
                PreComputedNearestPostcodesDto result = await connection.QuerySingleOrDefaultAsync<PreComputedNearestPostcodesDto>("[Address].[GetPreComputedNearestPostcodes]",
                    commandType: CommandType.StoredProcedure,
                    param: new { Postcode = postcode },
                    commandTimeout: 15);

                return result;
            }
        }


        public async Task<IEnumerable<PostcodeCoordinateDto>> GetPostcodeCoordinatesAsync(IEnumerable<string> postcodes)
        {
            DataTable postcodesDataTable = CreatePostcodeOnlyDataTable(postcodes);

            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.AddressService))
            {
                IEnumerable<PostcodeCoordinateDto> result = await connection.QueryAsync<PostcodeCoordinateDto>("[Address].[GetPostcodeCoordinates]",
                    commandType: CommandType.StoredProcedure,
                    param: new { Postcodes = postcodesDataTable },
                    commandTimeout: 15);

                return result;
            }
        }

        public async Task<IEnumerable<PostcodeCoordinateDto>> GetPostcodeCoordinatesAsync(int fromId, int toId)
        {
            string query = @"
SELECT [Postcode]
      ,[Latitude]
      ,[Longitude]
  FROM [AddressService].[Address].[Postcode]
  WHERE [IsActive]  = 1
  AND [Id] BETWEEN @fromId AND @toId
";

            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.AddressService))
            {
                IEnumerable<PostcodeCoordinateDto> result = await connection.QueryAsync<PostcodeCoordinateDto>(query,
                    commandType: CommandType.Text,
                    param: new { fromId = fromId, toId = toId },
                    commandTimeout: 15);

                return result;
            }
        }

        public async Task<int> GetMinPostcodeIdAsync()
        {
            int result = await _context.Set<PostcodeEntity>().Where(x=>x.IsActive).MinAsync(x => x.Id);
            return result;
        }

        public async Task<int> GetMaxPostcodeIdAsync()
        {
            int result = await _context.Set<PostcodeEntity>().Where(x => x.IsActive).MaxAsync(x => x.Id);
            return result;
        }

        public async Task<int> GetNumberOfPostcodesAsync()
        {
            int result = await _context.Set<PostcodeEntity>().CountAsync(x => x.IsActive);
            return result;
        }

        public async Task<IEnumerable<PostcodeWithNumberOfAddressesDto>> GetNumberOfAddressesPerPostcodeAsync(IEnumerable<string> postcodes)
        {
            DataTable postcodesDataTable = CreatePostcodeOnlyDataTable(postcodes);

            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.AddressService))
            {
                IEnumerable<PostcodeWithNumberOfAddressesDto> result = await connection.QueryAsync<PostcodeWithNumberOfAddressesDto>("[Address].[GetNumberOfAddressesPerPostcode]",
                    commandType: CommandType.StoredProcedure,
                    param: new { Postcodes = postcodesDataTable },
                    commandTimeout: 15);

                return result;
            }
        }
    }
}
