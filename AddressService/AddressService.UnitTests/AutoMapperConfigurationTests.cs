using AddressService.Mappers;
using AutoMapper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AddressService.UnitTests
{
    public  class AutoMapperConfigurationTests
    {
        [Test]
        public void AssertConfigurationIsValid()
        {
            IEnumerable<Type> autoMapperProfiles = typeof(PostCodeProfile).Assembly.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x));

            MapperConfiguration configuration = new MapperConfiguration(cfg =>
            {
                foreach (var profile in autoMapperProfiles)
                {
                    cfg.AddProfile(Activator.CreateInstance(profile) as Profile);
                }
            });

            configuration.AssertConfigurationIsValid();
        }
    }
}
