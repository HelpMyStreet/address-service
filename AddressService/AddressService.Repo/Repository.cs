using AddressService.Core.Config;
using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AutoMapper;
using Dapper;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Enums;
using System.Linq;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Newtonsoft.Json;
using HelpMyStreet.Contracts.AddressService.Request;

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
                if (connection.DataSource.Contains("database.windows.net"))
                {
                    connection.AccessToken = new AzureServiceTokenProvider().GetAccessTokenAsync("https://database.windows.net/").Result;
                }
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
                if (connection.DataSource.Contains("database.windows.net"))
                {
                    connection.AccessToken = new AzureServiceTokenProvider().GetAccessTokenAsync("https://database.windows.net/").Result;
                }
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
                if (connection.DataSource.Contains("database.windows.net"))
                {
                    connection.AccessToken = new AzureServiceTokenProvider().GetAccessTokenAsync("https://database.windows.net/").Result;
                }

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
                if (connection.DataSource.Contains("database.windows.net"))
                {
                    connection.AccessToken = new AzureServiceTokenProvider().GetAccessTokenAsync("https://database.windows.net/").Result;
                }

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
                if (connection.DataSource.Contains("database.windows.net"))
                {
                    connection.AccessToken = new AzureServiceTokenProvider().GetAccessTokenAsync("https://database.windows.net/").Result;
                }
                PreComputedNearestPostcodesDto result = await connection.QuerySingleOrDefaultAsync<PreComputedNearestPostcodesDto>("[Address].[GetPreComputedNearestPostcodes]",
                    commandType: CommandType.StoredProcedure,
                    param: new { Postcode = postcode },
                    commandTimeout: 15);

                return result;
            }
        }


        public async Task<IEnumerable<PostcodeWithCoordinatesDto>> GetPostcodeCoordinatesAsync(IEnumerable<string> postcodes)
        {
            DataTable postcodesDataTable = CreatePostcodeOnlyDataTable(postcodes);

            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.AddressService))
            {
                if (connection.DataSource.Contains("database.windows.net"))
                {
                    connection.AccessToken = new AzureServiceTokenProvider().GetAccessTokenAsync("https://database.windows.net/").Result;
                }
                IEnumerable<PostcodeWithCoordinatesDto> result = await connection.QueryAsync<PostcodeWithCoordinatesDto>("[Address].[GetPostcodeCoordinates]",
                    commandType: CommandType.StoredProcedure,
                    param: new { Postcodes = postcodesDataTable },
                    commandTimeout: 15);

                return result;
            }
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

        public async Task<IEnumerable<string>> GetPostcodesInBoundaryAsync(double swLatitude, double swLongitude, double neLatitude, double neLongitude)
        {
            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.AddressService))
            {
                IEnumerable<string> result = await connection.QueryAsync<string>("[Address].[GetPostcodesInBoundary]",
                    commandType: CommandType.StoredProcedure,
                    param: new { swLatitude = swLatitude, swLongitude = swLongitude, neLatitude = neLatitude , neLongitude = neLongitude },
                    commandTimeout: 15);

                return result;
            }
        }

        private LocationDetails MapEFLocationToLocationDetails(EntityFramework.Entities.Location location)
        {
            LocationInstructions locationInstructions = null;

            if (!String.IsNullOrEmpty(location.Instructions))
            {
                locationInstructions = JsonConvert.DeserializeObject<LocationInstructions>(location.Instructions);
            }

            return new LocationDetails()
            {
                Name = location.Name,
                ShortName = location.ShortName,
                Location = (Location) location.Id,
                LocationInstructions = locationInstructions,
                Address = new Address()
                {
                    AddressLine1 = location.AddressLine1,
                    AddressLine2 = location.AddressLine2,
                    AddressLine3 = location.AddressLine3,
                    Postcode = location.PostCode,
                    Locality = location.Locality
                },
                Latitude = location.Latitude,
                Longitude = location.Longitude
            };
        }

        public async Task<LocationDetails> GetLocationDetails(Location location)
        {
            int locationId = (int)location;
            var locationdetails = _context.Location.Where(x => x.Id == locationId).FirstOrDefault();

            if(locationdetails ==null)
            {
                throw new Exception($"Unable to retrieve location for location {location}");
            }

            return MapEFLocationToLocationDetails(locationdetails);
        }

        public List<LocationDetails> GetAllLocations()
        {
            List<LocationDetails> allLocations = new List<LocationDetails>();
            var locations = _context.Location.ToList();

            foreach(EntityFramework.Entities.Location l in locations)
            {
                allLocations.Add(MapEFLocationToLocationDetails(l));
            }

            return allLocations;
        }

        public async Task<List<LocationDetails>> GetLocations(List<LocationRequest> request)
        {
            var locations = request.Select(x => (int)x.Location).ToList();
            var locationdetails = _context.Location.Where(x => locations.Contains(x.Id)).ToList();

            if (locationdetails == null)
            {
                throw new Exception($"Unable to retrieve location for location");
            }

            List<LocationDetails> result = new List<LocationDetails>();

            foreach(EntityFramework.Entities.Location ld in locationdetails)
            {
                result.Add(MapEFLocationToLocationDetails(ld));
            }

            return result;
        }
    }
}
