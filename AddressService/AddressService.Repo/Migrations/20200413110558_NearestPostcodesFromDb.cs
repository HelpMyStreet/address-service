using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class NearestPostcodesFromDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROC [Address].[GetNearestPostcodes] 
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP PROC [Address].[GetNearestPostcodes] 
  ");
        }
    }
}
