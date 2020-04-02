using AddressService.Mappers;
using AutoMapper;
using NUnit.Framework;

namespace AddressService.UnitTests
{
    public  class AutoMapperTests
    {

        [Test]
        public void AssertConfigurationIsValid()
        {
            MapperConfiguration configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AddressDetailsProfile>();
                cfg.AddProfile<PostCodeProfile>();
            });

            configuration.AssertConfigurationIsValid();

        }
    }
}
