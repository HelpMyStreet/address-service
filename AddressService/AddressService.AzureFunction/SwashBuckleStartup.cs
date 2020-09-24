using System.Reflection;
using AddressService.AzureFunction;
using AzureFunctions.Extensions.Swashbuckle;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(SwashBuckleStartup))]
namespace AddressService.AzureFunction
{
    internal class SwashBuckleStartup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly());
        }
    }
}