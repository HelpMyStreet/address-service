using AddressService.AzureFunction;
using AzureFunctions.Extensions.Swashbuckle;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using System.Reflection;

[assembly: WebJobsStartup(typeof(SwashBuckleStartup))]
namespace AddressService.AzureFunction
{
    internal class SwashBuckleStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            //Register the extension
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly());

        }
    }
}
