using AddressService.Core.Config;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Core.Services.PostcodeIo;
using AddressService.Core.Services.Qas;
using AddressService.Core.Utils;
using AddressService.Core.Validation;
using AddressService.Handlers;
using AddressService.Mappers;
using AddressService.Repo;
using AutoMapper;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

[assembly: FunctionsStartup(typeof(AddressService.AzureFunction.Startup))]
namespace AddressService.AzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // We need to get the app directory this way.  Using Environment.CurrentDirectory doesn't work in Azure
            ExecutionContextOptions executioncontextoptions = builder.Services.BuildServiceProvider()
                .GetService<IOptions<ExecutionContextOptions>>().Value;
            string currentDirectory = executioncontextoptions.AppDirectory;

            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                .AddEnvironmentVariables();

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

            builder.Services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            builder.Services.TryAdd(ServiceDescriptor.Singleton(typeof(ILoggerWrapper<>), typeof(LoggerWrapper<>)));

            builder.Services.AddTransient<IHttpClientWrapper, HttpClientWrapper>();

            builder.Services.AddTransient<IQasMapper, QasMapper>();
            builder.Services.AddTransient<IQasService, QasService>();

            builder.Services.AddTransient<IPostcodeIoService, PostcodeIoService>();

            builder.Services.AddTransient<INearestPostcodeGetter, NearestPostcodeGetter>();
            builder.Services.AddTransient<IPostcodeGetter, PostcodeGetter>();

            builder.Services.AddTransient<IPostcodeCoordinatesGetter, PostcodeCoordinatesGetter>();

            builder.Services.AddTransient<IRegexPostcodeValidator, RegexPostcodeValidator>();
            builder.Services.AddTransient<IPostcodeValidator, PostcodeValidator>();

            builder.Services.AddTransient<IAddressDetailsSorter, AddressDetailsSorter>();

            builder.Services.AddTransient<IFriendlyNameGenerator, FriendlyNameGenerator>();

            builder.Services.AddMediatR(typeof(GetPostcodeHandler).Assembly);

            IEnumerable<Type> autoMapperProfiles = typeof(PostCodeProfile).Assembly.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x)).ToList();
            foreach (var profile in autoMapperProfiles)
            {
                builder.Services.AddAutoMapper(profile.Assembly);
            }

            builder.Services.AddTransient<IRepository, Repository>();


            IConfigurationSection applicationConfigSettings = config.GetSection("ApplicationConfig");
            builder.Services.Configure<ApplicationConfig>(applicationConfigSettings);

            IConfigurationSection connectionStringSettings = config.GetSection("ConnectionStrings");
            builder.Services.Configure<ConnectionStrings>(connectionStringSettings);

            ConnectionStrings connectionStrings = new ConnectionStrings();
            connectionStringSettings.Bind(connectionStrings);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    ConfigureDbContextOptionsBuilder(options, connectionStrings.AddressService),
                ServiceLifetime.Transient
            );

            // automatically apply EF migrations
            // DbContext is being created manually instead of through DI as it throws an exception and I've not managed to find a way to solve it yet: 
            // 'Unable to resolve service for type 'Microsoft.Azure.WebJobs.Script.IFileLoggingStatusManager' while attempting to activate 'Microsoft.Azure.WebJobs.Script.Diagnostics.HostFileLoggerProvider'.'
            DbContextOptionsBuilder<ApplicationDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            ConfigureDbContextOptionsBuilder(dbContextOptionsBuilder, connectionStrings.AddressService);
            ApplicationDbContext dbContext = new ApplicationDbContext(dbContextOptionsBuilder.Options);

            dbContext.Database.Migrate();

        }

        private void ConfigureDbContextOptionsBuilder(DbContextOptionsBuilder options, string connectionString)
        {
            options
                .UseSqlServer(connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }
}