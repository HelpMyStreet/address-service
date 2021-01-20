using AddressService.Repo.EntityFramework.Entities;
using HelpMyStreet.Utils.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace AddressService.Repo.Helpers
{
    public static class LocationExtensions
    {
        public static void SetLocations(this EntityTypeBuilder<Location> entity)
        {
            LocationInstructions location1 = new LocationInstructions()
            {
                Intro = "Location 1 intro",
                Steps = new System.Collections.Generic.List<Step>()
                {
                   new Step(){Heading = "Heading 1", Detail = "Detail 1"},
                   new Step(){Heading = "Heading 2", Detail = "Detail 2"}
                },
                Close = "Location 1 close"
            };

            LocationInstructions location2 = new LocationInstructions()
            {
                Intro = "Location 2 intro",
                Steps = new System.Collections.Generic.List<Step>()
                {
                   new Step(){Heading = "Heading 3", Detail = "Detail 3"},
                   new Step(){Heading = "Heading 4", Detail = "Detail 4"}
                },
                Close = "Location 2 close"
            };

            entity.HasData(new Location
            {
                Id = (int)HelpMyStreet.Utils.Enums.Location.Location1,
                Name = "Location 1",
                ShortName = "Short Location 1",
                AddressLine1 = "Age UK Lincoln & South Lincolnshire",
                AddressLine2 = "36 Park Street",
                AddressLine3 = "",
                Locality = "Lincoln",
                PostCode = "LN1 1UQ",
                Longitude = (decimal) -0.541420,
                Latitude = (decimal) 53.230492,
                Instructions = JsonConvert.SerializeObject(location1)
            });

            entity.HasData(new Location
            {
                Id = (int)HelpMyStreet.Utils.Enums.Location.Location2,
                Name = "Location 2",
                ShortName = "Short Location 2",
                AddressLine1 = "Location 2 Address Line 1",
                AddressLine2 = "Location 2 Address Line 2",
                AddressLine3 = "Location 2 Address Line 3",
                Locality = "Lincoln",
                PostCode = "LN1 1DD",
                Longitude = (decimal) -0.542170,
                Latitude = (decimal) 53.231289,
                Instructions = JsonConvert.SerializeObject(location2)
            });
        }
    }
}
