using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class UseDapper : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE TYPE [Address].[AddressDetail] AS TABLE(
	[AddressLine1] [varchar](100) NULL,
	[AddressLine2] [varchar](100) NULL,
	[AddressLine3] [varchar](100) NULL,
	[Locality] [varchar](100) NULL,
	[Postcode] [varchar](10) NOT NULL
)

CREATE TYPE [Address].[PostCode] AS TABLE(
	[Postcode] [varchar](10) NOT NULL,
	[LastUpdated] [datetime2](0) NOT NULL
)


CREATE TYPE [Address].[PostCodeOnly] AS TABLE(
	[Postcode] [varchar](10) NOT NULL
)
  ");

            migrationBuilder.Sql(@"
CREATE PROC [Address].[GetPostcodesAndAddresses] 
	@Postcodes [Address].[PostcodeOnly] READONLY
AS
SELECT 	pc.Id,
	pc.[Postcode],
	pc.[LastUpdated],	
	ad.[PostCodeId],
	ad.[Id],
	ad.[AddressLine1],
	ad.[AddressLine2],
	ad.[AddressLine3],
	pc.[Postcode],
	ad.[Locality]
FROM [Address].[AddressDetail] ad
INNER JOIN [Address].[PostCode] pc 
ON ad.PostCodeId = pc.Id
INNER JOIN @Postcodes p 
ON pc.Postcode = p.Postcode
  ");

            migrationBuilder.Sql(@"
CREATE PROC [Address].[SavePostcodesAndAddresses] 
	@Postcodes [Address].[Postcode] READONLY,
	@AddressDetails [Address].[AddressDetail] READONLY
AS
BEGIN TRANSACTION;

SAVE TRANSACTION PreInsert;

BEGIN TRY
	INSERT INTO [Address].[Postcode] (
		[Postcode],
		[LastUpdated]
		)
	SELECT [Postcode],
		[LastUpdated]
	FROM @Postcodes

	INSERT INTO [Address].[AddressDetail] (
		[PostcodeId],
		[AddressLine1],
		[AddressLine2],
		[AddressLine3],
		[Locality]
		)
	SELECT p.Id,
		ad.[AddressLine1],
		ad.[AddressLine2],
		ad.[AddressLine3],
		ad.[Locality]
	FROM @AddressDetails ad
	INNER JOIN [Address].[Postcode] p 
	ON ad.Postcode = p.Postcode

	COMMIT 

END TRY

BEGIN CATCH
	IF @@TRANCOUNT > 0
	BEGIN
		ROLLBACK TRANSACTION PreInsert;
	END;

	THROW

END CATCH
  ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP PROC [Address].[GetPostcodesAndAddresses]
DROP PROC [Address].[GetPostcodesAndAddresses]
  ");

            migrationBuilder.Sql(@"
DROP TYPE [Address].[AddressDetail]
DROP TYPE [Address].[PostCode] 
DROP TYPE [Address].[PostCodeOnly]
  ");
        }
    }
}
