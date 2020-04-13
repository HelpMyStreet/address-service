using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class NearestPostcodeCache : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreComputedNearestPostcodes",
                schema: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Postcode = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    CompressedNearestPostcodes = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreComputedNearestPostcodes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreComputedNearestPostcodes_Postcode",
                schema: "Address",
                table: "PreComputedNearestPostcodes",
                column: "Postcode",
                unique: true);


            migrationBuilder.Sql(@"
CREATE PROC [Address].[GetPreComputedNearestPostcodes] 
	@Postcode VARCHAR(10)
AS
SELECT [Id],
	[Postcode],
	[CompressedNearestPostcodes]
FROM [Address].[PreComputedNearestPostcodes]
WHERE [Postcode] = @Postcode
  ");

            migrationBuilder.Sql(@"
CREATE PROC [Address].[SavePreComputedNearestPostcodes] 
	@Postcode VARCHAR(10),
	@CompressedNearestPostcodes VARBINARY(MAX)
AS
INSERT INTO [Address].[PreComputedNearestPostcodes] (
	[Postcode],
	[CompressedNearestPostcodes]
	)
SELECT @Postcode,
	@CompressedNearestPostcodes
WHERE NOT EXISTS (
		SELECT *
		FROM [Address].[PreComputedNearestPostcodes]
		WHERE [Postcode] = @Postcode
		)
  ");


            migrationBuilder.Sql(@"
ALTER PROC [Address].[GetNearestPostcodes] 
@Postcode VARCHAR(10),
@DistanceInMetres FLOAT
AS
DECLARE @Latitude DECIMAL(9, 6);
DECLARE @Longitude DECIMAL(9, 6);

SELECT @Latitude = [Latitude],
	@Longitude = [Longitude]
FROM [Address].[Postcode]
WHERE [Postcode] = @postcode

DECLARE @point GEOGRAPHY = GEOGRAPHY::Point(@Latitude, @Longitude, 4326);

SELECT [Postcode],
	@point.STDistance([Coordinates]) AS [DistanceInMetres]
FROM [Address].[Postcode]
WHERE @point.STDistance([Coordinates]) <= @DistanceInMetres
AND [IsActive] = 1
  ");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
DROP PROC [Address].[GetPreComputedNearestPostcodes] 
DROP PROC [Address].[SavePreComputedNearestPostcodes]
  ");


            migrationBuilder.Sql(@"
ALTER PROC [Address].[GetNearestPostcodes] 
@Postcode VARCHAR(10),
@DistanceInMetres FLOAT,
@MaxNumberOfResults INT
AS
DECLARE @Latitude DECIMAL(9, 6);
DECLARE @Longitude DECIMAL(9, 6);

SELECT @Latitude = [Latitude],
	@Longitude = [Longitude]
FROM [Address].[Postcode]
WHERE [Postcode] = @postcode

DECLARE @point GEOGRAPHY = GEOGRAPHY::Point(@Latitude, @Longitude, 4326);

SELECT TOP(@MaxNumberOfResults) [Postcode],
	@point.STDistance([Coordinates]) AS [DistanceInMetres]
FROM [Address].[Postcode]
WHERE @point.STDistance([Coordinates]) <= @DistanceInMetres
AND [IsActive] = 1
ORDER BY @point.STDistance([Coordinates]) ASC
  ");

            migrationBuilder.DropTable(
                name: "PreComputedNearestPostcodes",
                schema: "Address");
        }
    }
}
