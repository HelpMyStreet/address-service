using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class DontSaveExistingPostcodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

						migrationBuilder.Sql(@"
ALTER PROC [Address].[SavePostcodesAndAddresses] 
	@Postcodes [Address].[Postcode] READONLY,
	@AddressDetails [Address].[AddressDetail] READONLY
AS
BEGIN TRANSACTION;

SAVE TRANSACTION PreInsert;

BEGIN TRY

	DECLARE @MissingPostcodes [Address].[PostCodeOnly]

	INSERT INTO @MissingPostcodes
	SELECT [Postcode]
	FROM @Postcodes p2
	WHERE NOT EXISTS(
	SELECT [Postcode]
	FROM [Address].[Postcode] p1
	WHERE p1.[Postcode] = p2.[Postcode]
	)

	INSERT INTO [Address].[Postcode] (
		[Postcode],
		[LastUpdated]
		)
	SELECT p.[Postcode],
		p.[LastUpdated]
	FROM @Postcodes p
	INNER JOIN @MissingPostcodes mp
	on p.[Postcode] = mp.[Postcode]

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
	INNER JOIN @MissingPostcodes mp
	on ad.[Postcode] = mp.[Postcode]

	COMMIT 

END TRY

BEGIN CATCH
	IF @@TRANCOUNT > 0
	BEGIN
		ROLLBACK TRANSACTION PreInsert;
	END;

	THROW

END CATCH
  
GO
  ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.Sql(@"
ALTER PROC [Address].[SavePostcodesAndAddresses] 
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
    }
}
