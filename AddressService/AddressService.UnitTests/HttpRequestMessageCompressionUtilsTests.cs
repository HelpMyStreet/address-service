using AddressService.Core.Utils;
using HelpMyStreet.Contracts.AddressService.Request;
using NUnit.Framework;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AddressService.UnitTests
{
    public class HttpRequestMessageCompressionUtilsTests
    {
        [Test]
        public async Task DeserialisationWhenContentIsCompressed()
        {
            var postcodeWithRadius = new PostcodeWithRadius()
            {
                Id = 1,
                Postcode = "NG1 5FS",
                RadiusInMetres = 100
            };

            var json = Utf8Json.JsonSerializer.ToJsonString(postcodeWithRadius);

            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            MemoryStream ms = new MemoryStream();
            using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                gzip.Write(jsonBytes, 0, jsonBytes.Length);
            }

            ms.Position = 0;
            StreamContent content = new StreamContent(ms);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.ContentEncoding.Add("gzip");


            var httpRequestMessage = new HttpRequestMessage()
            {
                Content = content
            };

            var result = await HttpRequestMessageCompressionUtils.DeserialiseAsync<PostcodeWithRadius>(httpRequestMessage);

            Assert.AreEqual(postcodeWithRadius.Id, result.Id);
            Assert.AreEqual(postcodeWithRadius.Postcode, result.Postcode);
            Assert.AreEqual(postcodeWithRadius.RadiusInMetres, result.RadiusInMetres);
        }

        [Test]
        public async Task DeserialisationWhenContentIsNotCompressed()
        {
            var postcodeWithRadius = new PostcodeWithRadius()
            {
                Id = 1,
                Postcode = "NG1 5FS",
                RadiusInMetres = 100
            };

            var json = Utf8Json.JsonSerializer.ToJsonString(postcodeWithRadius);


            var httpRequestMessage = new HttpRequestMessage()
            {
                Content = new StringContent(json)
            };

            var result = await HttpRequestMessageCompressionUtils.DeserialiseAsync<PostcodeWithRadius>(httpRequestMessage);

            Assert.AreEqual(postcodeWithRadius.Id, result.Id);
            Assert.AreEqual(postcodeWithRadius.Postcode, result.Postcode);
            Assert.AreEqual(postcodeWithRadius.RadiusInMetres, result.RadiusInMetres);
        }
    }
}
