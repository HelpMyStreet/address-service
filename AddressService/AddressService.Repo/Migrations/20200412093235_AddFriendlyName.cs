using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class AddFriendlyName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			//adding FriendlyName columns
            migrationBuilder.AddColumn<string>(
                name: "FriendlyName",
                schema: "Address",
                table: "Postcode",
                unicode: false,
                maxLength: 100,
                nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "FriendlyName",
				schema: "Staging",
				table: "Postcode_Staging",
				unicode: false,
				maxLength: 100,
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "FriendlyName",
				schema: "Staging",
				table: "Postcode_Switch",
				unicode: false,
				maxLength: 100,
				nullable: true);

			//update switch procedure to add copying friendly names
			migrationBuilder.Sql(@"
IF EXISTS (
		SELECT 1
		FROM sys.objects
		WHERE object_id = object_id('Staging.LoadFromStagingTableAndSwitch')
		)
BEGIN
	DROP PROCEDURE [Staging].[LoadFromStagingTableAndSwitch]
END
  ");

			migrationBuilder.Sql(@"
CREATE PROCEDURE [Staging].[LoadFromStagingTableAndSwitch]
AS
BEGIN
	SET XACT_ABORT ON
	SET	NOCOUNT ON

	BEGIN TRY
		TRUNCATE TABLE [Staging].[Postcode_Switch]

		-- make copy of table (the postcode tabble is not updated by the application so we won't lose any data)
		SET IDENTITY_INSERT [Staging].[Postcode_Switch] ON
		
		INSERT INTO [Staging].[Postcode_Switch] (
			[Id],
			[Postcode],
			[FriendlyName],
			[LastUpdated],
			[Latitude],
			[Longitude]
			)
		SELECT [Id],
			[Postcode],
			[FriendlyName],
			[LastUpdated],
			[Latitude],
			[Longitude]
		FROM [Address].[Postcode] WITH (NOLOCK)

		SET IDENTITY_INSERT [Staging].[Postcode_Switch] OFF

		-- update postcodes
		MERGE [Staging].[Postcode_Switch] sw
		USING [Staging].[Postcode_Staging] st
			ON (sw.[Postcode] = st.[Postcode])
		WHEN MATCHED
			AND (
				sw.[Latitude] != st.[Latitude]
				OR sw.[Longitude] != st.[Longitude]
				)
			THEN
				UPDATE
				SET sw.[Latitude] = st.[Latitude],
					sw.[Longitude] = st.[Longitude],
					sw.[LastUpdated] = GetUtcDate()
		--note: when not matched don't need to insert friendly name - it won't have been generated yet and should be null
		WHEN NOT MATCHED BY TARGET
			THEN
				INSERT (
					[Postcode],
					[LastUpdated],
					[Latitude],
					[Longitude]
					)
				VALUES (
					st.[Postcode],
					GetUtcDate(),
					st.[Latitude],
					st.[Longitude]
					);

		BEGIN TRANSACTION T1

		-- swap updated Postode_Switch table with Postcode table

		-- should be fine to drop constraint as addresses will only be added if the postcode is in [Address].[Postcode] and we have just copied that table, but we'll take a table lock anyway
		SELECT TOP 1 [Id] FROM  [Address].[AddressDetail] WITH (TABLOCKX, HOLDLOCK)
		
		ALTER TABLE [Address].[AddressDetail] DROP CONSTRAINT [FK_AddressDetails_Address_Postcode]

		-- can't use a switch statement because [Address].[Postcode] has a spatial index
		EXEC sp_rename '[Address].[Postcode]',
			'Postcode_Old',
			'object'

		EXEC sp_rename N'[Address].[Postcode_Old].[PK_Postcode]',
			N'PK_Postcode_Old'

		EXEC sp_rename '[Staging].[Postcode_Switch]',
			'Postcode',
			'object'

		ALTER SCHEMA [Address] TRANSFER [Staging].[Postcode]

		EXEC sp_rename N'[Address].[Postcode].[PK_Postcode_Switch]',
			N'PK_Postcode'

		EXEC sp_rename '[Address].[Postcode_Old]',
			'Postcode_Switch',
			'object'

		ALTER SCHEMA [Staging] TRANSFER [Address].[Postcode_Switch]

		EXEC sp_rename N'[Staging].[Postcode_Switch].[PK_Postcode_Old]',
			N'PK_Postcode_Switch'

		-- takes 2 seconds with 30m address rows on my laptop, so not too bad considering this should be run every 3 months the ONS release a new postcode file!
		ALTER TABLE [Address].[AddressDetail]  WITH CHECK ADD  CONSTRAINT [FK_AddressDetails_Address_Postcode] FOREIGN KEY([PostcodeId])
		REFERENCES [Address].[Postcode] ([Id])
		ON DELETE CASCADE

		COMMIT
	END TRY

	BEGIN CATCH
		IF @@trancount > 0
			ROLLBACK ;
			THROW;
	END CATCH
END

  ");



			//Alter GetPostcodesAndAddresses to add friendly name
			migrationBuilder.Sql(@"
IF EXISTS (
		SELECT 1
		FROM sys.objects
		WHERE object_id = object_id('Address.GetPostcodesAndAddresses')
		)
BEGIN
	DROP PROCEDURE [Address].[GetPostcodesAndAddresses]
END
  ");

			migrationBuilder.Sql(@"
CREATE PROC [Address].[GetPostcodesAndAddresses] 
	@Postcodes [Address].[PostcodeOnly] READONLY
AS
SELECT 	pc.Id,
	pc.[Postcode],
	pc.[FriendlyName],
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

			////adding new type
			migrationBuilder.Sql(@"IF TYPE_ID(N'[Address].[FriendlyName]') IS NOT NULL
	DROP TYPE [Address].[FriendlyName]");


			migrationBuilder.Sql(@"CREATE TYPE [Address].[FriendlyName] AS TABLE(
	[Postcode] [varchar](10) NOT NULL,
	[FriendlyName] [varchar](100) NULL
	)");


			//adding new SP to populate
			migrationBuilder.Sql(@"
IF EXISTS (
		SELECT 1
		FROM sys.objects
		WHERE object_id = object_id('Address.SaveFriendlyNames')
		)
BEGIN
	DROP PROCEDURE [Address].[SaveFriendlyNames]
END
  ");


			migrationBuilder.Sql(@"CREATE PROCEDURE [Address].[SaveFriendlyNames]
	@FriendlyNames [Address].[FriendlyName] READONLY
AS
MERGE INTO [Address].[Postcode] P
	USING @FriendlyNames F
	ON P.Postcode = F.Postcode
WHEN MATCHED THEN
	UPDATE
	SET P.FriendlyName = F.FriendlyName;") ;
			

		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			//drop friendlyname columns
            migrationBuilder.DropColumn(
                name: "FriendlyName",
                schema: "Address",
                table: "Postcode");

			migrationBuilder.DropColumn(
				name: "FriendlyName",
				schema: "Staging",
				table: "Postcode_Staging");

			migrationBuilder.DropColumn(
				name: "FriendlyName",
				schema: "Staging",
				table: "Postcode_Switch");





			//update switch procedure to remove friendly names
			migrationBuilder.Sql(@"
IF EXISTS (
		SELECT 1
		FROM sys.objects
		WHERE object_id = object_id('Staging.LoadFromStagingTableAndSwitch')
		)
BEGIN
	DROP PROCEDURE [Staging].[LoadFromStagingTableAndSwitch]
END
  ");

			migrationBuilder.Sql(@"
CREATE PROCEDURE [Staging].[LoadFromStagingTableAndSwitch]
AS
BEGIN
	SET XACT_ABORT ON
	SET	NOCOUNT ON

	BEGIN TRY
		TRUNCATE TABLE [Staging].[Postcode_Switch]

		-- make copy of table (the postcode tabble is not updated by the application so we won't lose any data)
		SET IDENTITY_INSERT [Staging].[Postcode_Switch] ON
		
		INSERT INTO [Staging].[Postcode_Switch] (
			[Id],
			[Postcode],
			[LastUpdated],
			[Latitude],
			[Longitude]
			)
		SELECT [Id],
			[Postcode],
			[LastUpdated],
			[Latitude],
			[Longitude]
		FROM [Address].[Postcode] WITH (NOLOCK)

		SET IDENTITY_INSERT [Staging].[Postcode_Switch] OFF

		-- update postcodes
		MERGE [Staging].[Postcode_Switch] sw
		USING [Staging].[Postcode_Staging] st
			ON (sw.[Postcode] = st.[Postcode])
		WHEN MATCHED
			AND (
				sw.[Latitude] != st.[Latitude]
				OR sw.[Longitude] != st.[Longitude]
				)
			THEN
				UPDATE
				SET sw.[Latitude] = st.[Latitude],
					sw.[Longitude] = st.[Longitude],
					sw.[LastUpdated] = GetUtcDate()
		WHEN NOT MATCHED BY TARGET
			THEN
				INSERT (
					[Postcode],
					[LastUpdated],
					[Latitude],
					[Longitude]
					)
				VALUES (
					st.[Postcode],
					GetUtcDate(),
					st.[Latitude],
					st.[Longitude]
					);

		BEGIN TRANSACTION T1

		-- swap updated Postode_Switch table with Postcode table

		-- should be fine to drop constraint as addresses will only be added if the postcode is in [Address].[Postcode] and we have just copied that table, but we'll take a table lock anyway
		SELECT TOP 1 [Id] FROM  [Address].[AddressDetail] WITH (TABLOCKX, HOLDLOCK)
		
		ALTER TABLE [Address].[AddressDetail] DROP CONSTRAINT [FK_AddressDetails_Address_Postcode]

		-- can't use a switch statement because [Address].[Postcode] has a spatial index
		EXEC sp_rename '[Address].[Postcode]',
			'Postcode_Old',
			'object'

		EXEC sp_rename N'[Address].[Postcode_Old].[PK_Postcode]',
			N'PK_Postcode_Old'

		EXEC sp_rename '[Staging].[Postcode_Switch]',
			'Postcode',
			'object'

		ALTER SCHEMA [Address] TRANSFER [Staging].[Postcode]

		EXEC sp_rename N'[Address].[Postcode].[PK_Postcode_Switch]',
			N'PK_Postcode'

		EXEC sp_rename '[Address].[Postcode_Old]',
			'Postcode_Switch',
			'object'

		ALTER SCHEMA [Staging] TRANSFER [Address].[Postcode_Switch]

		EXEC sp_rename N'[Staging].[Postcode_Switch].[PK_Postcode_Old]',
			N'PK_Postcode_Switch'

		-- takes 2 seconds with 30m address rows on my laptop, so not too bad considering this should be run every 3 months the ONS release a new postcode file!
		ALTER TABLE [Address].[AddressDetail]  WITH CHECK ADD  CONSTRAINT [FK_AddressDetails_Address_Postcode] FOREIGN KEY([PostcodeId])
		REFERENCES [Address].[Postcode] ([Id])
		ON DELETE CASCADE

		COMMIT
	END TRY

	BEGIN CATCH
		IF @@trancount > 0
			ROLLBACK ;
			THROW;
	END CATCH
END

  ");



			//Alter GetPostcodesAndAddresses to remove friendly name
			migrationBuilder.Sql(@"
IF EXISTS (
		SELECT 1
		FROM sys.objects
		WHERE object_id = object_id('Address.GetPostcodesAndAddresses')
		)
BEGIN
	DROP PROCEDURE [Address].[GetPostcodesAndAddresses]
END
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


			//drop new SP to populate friendly names
			migrationBuilder.Sql(@"
IF EXISTS (
		SELECT 1
		FROM sys.objects
		WHERE object_id = object_id('Address.SaveFriendlyNames')
		)
BEGIN
	DROP PROCEDURE [Address].[SaveFriendlyNames]
END
  ");
			//drop new type
			migrationBuilder.Sql(@"IF TYPE_ID(N'[Address].[FriendlyName]') IS NOT NULL
	DROP TYPE [Address].[FriendlyName]");
		}

    }
}
