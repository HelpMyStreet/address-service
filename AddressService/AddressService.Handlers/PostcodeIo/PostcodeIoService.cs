using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AddressService.Core.Config;
using AddressService.Core.Dto;
using AddressService.Core.Utils;
using Marvin.StreamExtensions;

namespace AddressService.Handlers.PostcodeIo
{
    public class PostcodeIoService : IPostcodeIoService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public PostcodeIoService(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<PostCodeIoNearestRootResponse> GetNearbyPostCodesAsync(string postcode)
        {
            string path = $"postcodes/{postcode}/nearest";
            // 804km = 0.5 miles
            // todo limiting QAS results to 5 until addresses are stored in DB
            string query = "limit=5&radius=804.672"; 
            string absolutePath = $"{path}?{query}";

            PostCodeIoNearestRootResponse postCodeIoNearestRootResponse;

            using (HttpResponseMessage response = await _httpClientWrapper.GetAsync(HttpClientConfigName.PostcodeIo, absolutePath).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                Stream stream = await response.Content.ReadAsStreamAsync();
                postCodeIoNearestRootResponse = stream.ReadAndDeserializeFromJson<PostCodeIoNearestRootResponse>();
            }

            return postCodeIoNearestRootResponse;
        }
    }
}
