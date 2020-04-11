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

        public async Task SaveAddressesAsync(IEnumerable<PostcodeDto> postcodes)
        {
            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.AddressService))
            {
                DataTable addressDataTable = CreateAddressDataTable(postcodes);

                await connection.ExecuteAsync("[Address].[SaveAddresses]",
                   commandType: CommandType.StoredProcedure,
                   param: new {  AddressDetails = addressDataTable },
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
    }
}
