using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Config;
using AddressService.Core.Dto;
using AddressService.Core.Utils;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Utils;
using Marvin.StreamExtensions;

namespace AddressService.Core.Services.PostcodeIo
{
    public class PostcodeIoService : IPostcodeIoService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public PostcodeIoService(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<PostCodeIoNearestRootResponse> GetNearbyPostCodesAsync(string postcode, CancellationToken cancellationToken)
        {
            string path = $"postcodes/{postcode}/nearest";
            // 804.672km = 0.5 miles
            string query = "limit=100&radius=805"; 
            string absolutePath = $"{path}?{query}";

            PostCodeIoNearestRootResponse postCodeIoNearestRootResponse;

            using (HttpResponseMessage response = await _httpClientWrapper.GetAsync(HttpClientConfigName.PostcodeIo, absolutePath, cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                Stream stream = await response.Content.ReadAsStreamAsync();
                postCodeIoNearestRootResponse = stream.ReadAndDeserializeFromJson<PostCodeIoNearestRootResponse>();
            }

            return postCodeIoNearestRootResponse;
        }


        public async Task<bool> IsPostcodeValidAsync(string postcode, CancellationToken cancellationToken)
        {
            string path = $"postcodes/{postcode}/validate";
            string absolutePath = $"{path}";

            PostCodeIoValidPostcodeRootResponse postCodeIoValidPostcodeRootResponse;

            using (HttpResponseMessage response = await _httpClientWrapper.GetAsync(HttpClientConfigName.PostcodeIo, absolutePath, cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                Stream stream = await response.Content.ReadAsStreamAsync();
                postCodeIoValidPostcodeRootResponse = stream.ReadAndDeserializeFromJson<PostCodeIoValidPostcodeRootResponse>();
            }

            return postCodeIoValidPostcodeRootResponse.Result;
        }
    }
}
