using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Config;

namespace AddressService.Core.Utils
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> GetAsync(HttpClientConfigName httpClientConfigName, string absolutePath, CancellationToken cancellationToken);
        Task PostAsync(HttpClientConfigName httpClientConfigName, string absolutePath, HttpContent content, CancellationToken cancellationToken);
    }
}