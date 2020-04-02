using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using AutoMapper;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Handlers;
using AddressService.Mappers;
using AddressService.Repo;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using AddressService.Core.Config;
using AddressService.Core.Services.PostcodeIo;
using AddressService.Core.Services.Qas;
using AddressService.Core.Utils;
using AddressService.Core.Validation;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(AddressService.AzureFunction.Startup))]
namespace AddressService.AzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("local.settings.json", true, reloadOnChange: true)
                .AddEnvironmentVariables();

            string aspNetCoreEnv = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            bool isLocalDev = aspNetCoreEnv?.ToLower().Trim() == "localdev";
            if (isLocalDev)
            {
                configBuilder.AddUserSecrets(Assembly.GetExecutingAssembly(), false);
                Console.Write("User secrets added");
            }
            else
            {
                Console.Write("User secrets not added as ASPNETCORE_ENVIRONMENT environment variable doesn't contain \"localdev\"");
            }

            IConfigurationRoot config = configBuilder.Build();


            Dictionary<HttpClientConfigName, ApiConfig> httpClientConfigs = config.GetSection("Apis").Get<Dictionary<HttpClientConfigName, ApiConfig>>();

            foreach (KeyValuePair<HttpClientConfigName, ApiConfig> httpClientConfig in httpClientConfigs)
            {
                if (httpClientConfig.Key == HttpClientConfigName.Qas && !httpClientConfig.Value.Headers.ContainsKey("auth-token"))
                {
                    throw new Exception("Qas requires auth-token header");
                }

                builder.Services.AddHttpClient(httpClientConfig.Key.ToString(), c =>
                {
                    c.BaseAddress = new Uri(httpClientConfig.Value.BaseAddress);

                    c.Timeout = httpClientConfig.Value.Timeout ?? new TimeSpan(0, 0, 0, 15);

                    foreach (KeyValuePair<string, string> header in httpClientConfig.Value.Headers)
                    {
                        c.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    c.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    c.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

                }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    MaxConnectionsPerServer = httpClientConfig.Value.MaxConnectionsPerServer ?? 15,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                });

            }

            builder.Services.AddTransient<IHttpClientWrapper, HttpClientWrapper>();

            builder.Services.AddTransient<IQasMapper, QasMapper>();
            builder.Services.AddTransient<IQasService, QasService>();

            builder.Services.AddTransient<IPostcodeIoService, PostcodeIoService>();

            builder.Services.AddTransient<IPostcodeGetter, PostcodeGetter>();

            builder.Services.AddTransient<IPostcodeValidator, PostcodeValidator>();

            builder.Services.AddMediatR(typeof(GetPostcodeHandler).Assembly);
            builder.Services.AddMediatR(typeof(GetNearbyPostcodesHandler).Assembly);

            IEnumerable<Type> autoMapperProfiles = typeof(PostCodeProfile).Assembly.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x)).ToList();
            foreach (var profile in autoMapperProfiles)
            {
                builder.Services.AddAutoMapper(profile.Assembly);
            }

            builder.Services.AddTransient<IRepository, Repository>();


            IConfigurationSection applicationConfigSettings = config.GetSection("ApplicationConfig");
            builder.Services.Configure<ApplicationConfig>(applicationConfigSettings);

            string connectionStringSection = isLocalDev ? "ConnectionStringsLocalDev" : "ConnectionStrings";
            IConfigurationSection connectionStringSettings = config.GetSection(connectionStringSection);
            builder.Services.Configure<ConnectionStrings>(connectionStringSettings);

            ConnectionStrings connectionStrings = new ConnectionStrings();
            connectionStringSettings.Bind(connectionStrings);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options
                        .UseSqlServer(connectionStrings.AddressService)
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking),
                ServiceLifetime.Transient
            );
        }
    }
}