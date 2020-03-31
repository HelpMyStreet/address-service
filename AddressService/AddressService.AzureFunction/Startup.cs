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
using System;
using System.Configuration;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(AddressService.AzureFunction.Startup))]
namespace AddressService.AzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddMediatR(typeof(GetPostCodeHandler).Assembly);
            builder.Services.AddAutoMapper(typeof(AddressDetailsProfile).Assembly);

            var tmpConfig = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json",true)
            .Build();

            var sqlConnectionString = tmpConfig.GetConnectionString("SqlConnectionString");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(sqlConnectionString));

            builder.Services.AddTransient<IRepository, Repository>();
        }
    }
}