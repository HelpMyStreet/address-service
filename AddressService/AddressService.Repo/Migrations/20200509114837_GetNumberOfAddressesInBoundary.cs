using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class GetNumberOfAddressesInBoundary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // querying this index to find postcodes within a boundary is much faster than using  spacial index on the [Coordinates] column
            migrationBuilder.CreateIndex(
                name: "IXF_Postcode_Latitude_Longitude",
                schema: "Address",
                table: "Postcode",
                columns: new[] { "Latitude", "Longitude" },
                filter: "[IsActive] = 1")
                .Annotation("SqlServer:Include", new[] { "Postcode" });

            migrationBuilder.Sql(@"
CREATE PROC [Address].[GetPostcodesInBoundary]
@swLatitude DECIMAL(9,6),
@neLatitude DECIMAL(9,6),
@swLongitude DECIMAL(9,6),
@neLongitude DECIMAL(9,6)
AS
SELECT [Postcode]
  FROM [Address].[Postcode]
  where [Latitude] >= @swLatitude
  AND [Latitude] <= @neLatitude
  AND [Longitude] >= @swLongitude
  AND [Longitude] <= @neLongitude
  AND [IsActive] = 1
  ");

            migrationBuilder.Sql(@"
CREATE PROC [Address].[GetNumberOfAddressesPerPostcode] 
	@Postcodes [Address].[PostcodeOnly] READONLY
AS
SELECT 	pc.[Postcode],
	COUNT(*) AS [NumberOfAddresses]
FROM [Address].[AddressDetail] ad
INNER JOIN [Address].[PostCode] pc 
ON ad.PostCodeId = pc.Id
INNER JOIN @Postcodes p 
ON pc.Postcode = p.Postcode
GROUP BY pc.[Postcode]  
  ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IXF_Postcode_Latitude_Longitude",
                schema: "Address",
                table: "Postcode");

            migrationBuilder.Sql(@"
DROP PROC [Address].[GetNumberOfAddressesPerPostcode] 
  ");

            migrationBuilder.Sql(@"
DROP PROC [Address].[GetPostcodesInBoundary] 
  ");

        }
    }
}
