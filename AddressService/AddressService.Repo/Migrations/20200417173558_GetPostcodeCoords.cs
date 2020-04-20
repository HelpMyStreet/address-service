using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class GetPostcodeCoords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IXF_Postcode_Postcode",
                schema: "Address",
                table: "Postcode",
                column: "Postcode",
                unique: true,
                filter: "[IsActive] = 1")
                .Annotation("SqlServer:Include", new[] { "Latitude", "Longitude" });

            migrationBuilder.Sql(@"
CREATE PROC [Address].[GetPostcodeCoordinates]
@Postcodes [Address].[PostcodeOnly] READONLY
AS

SELECT p1.[Postcode]
      ,p1.[Latitude]
      ,p1.[Longitude]
  FROM [Address].[Postcode] p1
INNER JOIN @Postcodes p2 
ON p1.[Postcode] = p2.[Postcode]
WHERE [IsActive] = 1
  ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP PROC [Address].[GetPostcodeCoordinates]
  ");
            migrationBuilder.DropIndex(
                name: "IXF_Postcode_Postcode",
                schema: "Address",
                table: "Postcode");

        }
    }
}
