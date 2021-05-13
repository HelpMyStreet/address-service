using AddressService.Repo.EntityFramework.Entities;
using HelpMyStreet.Utils.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace AddressService.Repo.Helpers
{
    public static class LocationExtensions
    {

        private static LocationInstructions GetDummyInstructions()
        {
            return new LocationInstructions()
            {
                Intro = "Location intro",
                Steps = new System.Collections.Generic.List<Step>()
                {
                   new Step(){Heading = "Heading 1", Detail = "Detail 1"},
                   new Step(){Heading = "Heading 2", Detail = "Detail 2"}
                },
                Close = "Location close"
            };
        }

        private static LocationInstructions GetApexPCNInstructions()
        {
            return new LocationInstructions()
            {                
                Steps = new System.Collections.Generic.List<Step>()
                {
                   new Step()
                   {
                       Heading = "Information", 
                       Detail = "Please make sure to arrive 15 minutes before the start of your shift and bring clothing " +
                       "appropriate for the weather on the day as you may be asked to spend some time outside during your shift."
                    },             
                }                
            };
        }

        public static void SetLocations(this EntityTypeBuilder<Location> entity)
        {
            entity.HasData(new Location
            {
                Id = (int)HelpMyStreet.Utils.Enums.Location.LincolnCountyHospital,
                Name = "Lincoln County Hospital",
                ShortName = "Lincoln County Hospital",
                AddressLine1 = "Greetwell Road",
                AddressLine2 = "Lincoln",
                AddressLine3 = "",
                Locality = "Lincolnshire",
                PostCode = "LN2 5QY",
                Longitude = (decimal)-0.514990,
                Latitude = (decimal) 53.234482,
                Instructions = JsonConvert.SerializeObject(GetDummyInstructions())
            });

            entity.HasData(new Location
            {
                Id = (int)HelpMyStreet.Utils.Enums.Location.PilgramHospitalBolton,
                Name = "Pilgrim Hospital, Boston",
                ShortName = "Boston (Pilgrim Hospital)",
                AddressLine1 = "Sibsey Road",
                AddressLine2 = "Boston",
                AddressLine3 = "",
                Locality = "Lincolnshire",
                PostCode = "PE21 9QS",
                Longitude = (decimal) -0.006840,
                Latitude = (decimal) 52.993149,
                Instructions = JsonConvert.SerializeObject(GetDummyInstructions())
            });

            entity.HasData(new Location
            {
                Id = (int)HelpMyStreet.Utils.Enums.Location.LouthCommunityHospital,
                Name = "Louth Community Hospital",
                ShortName = "Louth",
                AddressLine1 = "High Holme Rd",
                AddressLine2 = "Louth",
                AddressLine3 = "",
                Locality = "Lincolnshire",
                PostCode = "LN11 0EU",
                Longitude = (decimal)-0.004510,
                Latitude = (decimal)53.371208,
                Instructions = JsonConvert.SerializeObject(GetDummyInstructions())
            });

            entity.HasData(new Location
            {
                Id = (int)HelpMyStreet.Utils.Enums.Location.TableTennisClubGrantham,
                Name = "Table Tennis Club, Grantham",
                ShortName = "Grantham",
                AddressLine1 = "Grantham Meres Leisure Centre Table Tennis Club",
                AddressLine2 = "Grantham Meres Leisure Centre",
                AddressLine3 = "Trent Road",
                Locality = "Grantham",
                PostCode = "NG31 7XQ",
                Longitude = (decimal)-0.660450,
                Latitude = (decimal)52.903179,
                Instructions = JsonConvert.SerializeObject(GetDummyInstructions())
            });

            entity.HasData(new Location
            {
                Id = (int)HelpMyStreet.Utils.Enums.Location.WaddingtonBranchSurgerySouthLincoln,
                Name = "Waddington Branch Surgery, South Lincoln",
                ShortName = "Lincoln South (Waddington Branch Surgery)",
                AddressLine1 = "Cliff Villages Medical Practice",
                AddressLine2 = "Mere Rd",
                AddressLine3 = "Waddington",
                Locality = "Lincoln",
                PostCode = "LN5 9NX",
                Longitude = (decimal)-0.535592,
                Latitude = (decimal)53.165936,
                Instructions = JsonConvert.SerializeObject(GetDummyInstructions())
            });

            entity.HasData(new Location
            {
                Id = (int)HelpMyStreet.Utils.Enums.Location.StMarysMedicalPracticeStamford,
                Name = "St. Mary’s Medical Centre, Stamford",
                ShortName = "Stamford (St. Mary’s Medical Centre)",
                AddressLine1 = "St. Mary’s Medical Centre",
                AddressLine2 = "Wharf Road",
                AddressLine3 = "Stamford",
                Locality = "Lincolnshire",
                PostCode = "PE9 2DH",
                Longitude = (decimal)-0.477465,
                Latitude = (decimal)52.650925,
                Instructions = JsonConvert.SerializeObject(GetDummyInstructions())
            });

            entity.HasData(new Location
            {
                Id = (int)HelpMyStreet.Utils.Enums.Location.FranklinHallSpilsby,
                Name = "Franklin Hall, Spilsby",
                ShortName = "Spilsby",
                AddressLine1 = "Franklin Hall",
                AddressLine2 = "Halton Road",
                AddressLine3 = "Spilsby",
                Locality = "",
                PostCode = "PE23 5LA",
                Longitude = (decimal)0.099136,
                Latitude = (decimal)53.172300,
                Instructions = JsonConvert.SerializeObject(GetDummyInstructions())
            });

            entity.HasData(new Location
            {
                Id = (int)HelpMyStreet.Utils.Enums.Location.SidingsMedicalPracticeBoston,
                Name = "Sidings Medical Practice, Boston",
                ShortName = "Boston (Sidings Medical Practice)",
                AddressLine1 = "Sidings Medical Practice",
                AddressLine2 = "14 Sleaford Rd",
                AddressLine3 = "Boston",
                Locality = "",
                PostCode = "PE21 8EG",
                Longitude = (decimal)-0.033522,
                Latitude = (decimal)52.975942,
                Instructions = JsonConvert.SerializeObject(GetDummyInstructions())
            });

            entity.HasData(new Location
            {
                Id = (int)HelpMyStreet.Utils.Enums.Location.RustonsSportsAndSocialClubLincoln,
                Name = "Ruston Sports and Social Club, Lincoln",
                ShortName = "Lincoln (Ruston Sports and Social Club)",
                AddressLine1 = "Ruston Sports & Social Club",
                AddressLine2 = "Newark Road",
                AddressLine3 = "Lincoln",
                Locality = "",
                PostCode = "LN6 8RN",
                Longitude = (decimal)-0.574294,
                Latitude = (decimal)53.196498,
                Instructions = JsonConvert.SerializeObject(GetApexPCNInstructions())
            });

            entity.HasData(new Location
            {
                Id = (int)HelpMyStreet.Utils.Enums.Location.PortlandMedicalPracticeLincoln,
                Name = "Portland Medical Practice, Lincoln",
                ShortName = "Lincoln (Portland Medical Practice)",
                AddressLine1 = "Portland Medical Practice",
                AddressLine2 = "60 Portland St",
                AddressLine3 = "Lincoln",
                Locality = "",
                PostCode = "LN5 7LB",
                Longitude = (decimal)-0.539074,
                Latitude = (decimal)53.223720,
                Instructions = JsonConvert.SerializeObject(GetDummyInstructions())
            });

        }
    }
}
