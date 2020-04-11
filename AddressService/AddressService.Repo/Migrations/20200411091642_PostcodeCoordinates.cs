using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class PostcodeCoordinates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {





            // add staging and switch tables
            migrationBuilder.Sql(@"

IF NOT EXISTS (
		SELECT SCHEMA_ID
		FROM sys.schemas
		WHERE [name] = 'Staging'
		)
BEGIN
	EXEC ('CREATE SCHEMA [Staging]');
END

IF NOT EXISTS (
		SELECT *
		FROM sys.tables t
		JOIN sys.schemas s ON (t.schema_id = s.schema_id)
		WHERE s.name = 'Staging'
			AND t.name = 'Postcode_Staging'
		)
BEGIN
	CREATE TABLE [Staging].[Postcode_Staging] (
		[Id] [int] IDENTITY(1, 1) NOT NULL,
		[Postcode] [varchar](10) NOT NULL,
		[Latitude] [decimal](9, 6) NOT NULL,
		[Longitude] [decimal](9, 6) NOT NULL,
		CONSTRAINT [PK_Postcode_Staging] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (
			PAD_INDEX = OFF,
			STATISTICS_NORECOMPUTE = OFF,
			IGNORE_DUP_KEY = OFF,
			ALLOW_ROW_LOCKS = ON,
			ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
		) ON [PRIMARY]
END

IF NOT EXISTS (
		SELECT *
		FROM sys.tables t
		JOIN sys.schemas s ON (t.schema_id = s.schema_id)
		WHERE s.name = 'Staging'
			AND t.name = 'Postcode_Switch'
		)
BEGIN
	CREATE TABLE [Staging].[Postcode_Switch] (
		[Id] [int] IDENTITY(1, 1) NOT NULL,
		[Postcode] [varchar](10) NOT NULL,
		[LastUpdated] [datetime2](0) NOT NULL,
		[Latitude] [decimal](9, 6) NOT NULL,
		[Longitude] [decimal](9, 6) NOT NULL,
		[Coordinates] AS ([geography]::Point([Latitude], [Longitude], (4326))) PERSISTED,
		CONSTRAINT [PK_Postcode_Switch] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (
			PAD_INDEX = OFF,
			STATISTICS_NORECOMPUTE = OFF,
			IGNORE_DUP_KEY = OFF,
			ALLOW_ROW_LOCKS = ON,
			ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
		) ON [PRIMARY]
		
CREATE SPATIAL INDEX [IX_Coordinates] ON [Staging].[Postcode_Switch]
(
	[Coordinates]
)USING  GEOGRAPHY_AUTO_GRID 
WITH (
CELLS_PER_OBJECT = 12, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


CREATE UNIQUE NONCLUSTERED INDEX [IX_Postcode_Postcode] ON [Staging].[Postcode_Switch]
(
	[Postcode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END


  ");




            // Delete all data first
            migrationBuilder.Sql(@"
  DELETE FROM [Address].[Postcode]

  TRUNCATE TABLE [Address].[AddressDetail]

  DBCC CHECKIDENT ('[Address].[Postcode]', RESEED, 0);
  ");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdated",
                schema: "Address",
                table: "Postcode",
                type: "datetime2(0)",
                nullable: false,
                defaultValueSql: "GetUtcDate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(0)");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                schema: "Address",
                table: "Postcode",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                schema: "Address",
                table: "Postcode",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                schema: "Address",
                table: "AddressDetail",
                type: "datetime2(0)",
                nullable: false,
                defaultValueSql: "GetUtcDate()");


            // Net Core 2.1 doesn't support spacial types :(
            migrationBuilder.Sql(@"
alter table [Address].[Postcode] add [Coordinates] as geography::Point(Latitude, Longitude, 4326) persisted;

create spatial index [IX_Coordinates] on [Address].[Postcode] ([Coordinates])
  ");


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


            migrationBuilder.Sql(@"
DROP PROC [Address].[SavePostcodesAndAddresses]
  ");

            migrationBuilder.Sql(@"
DROP TYPE [Address].[AddressDetail]
  ");

            migrationBuilder.Sql(@"
CREATE TYPE [Address].[AddressDetail] AS TABLE(
	[AddressLine1] [varchar](100) NULL,
	[AddressLine2] [varchar](100) NULL,
	[AddressLine3] [varchar](100) NULL,
	[Locality] [varchar](100) NULL,
	[Postcode] [varchar](10) NOT NULL,
	[LastUpdated] [datetime2](0) NOT NULL
)
  ");

						migrationBuilder.Sql(@"
CREATE PROC [Address].[SaveAddresses] 
	@AddressDetails [Address].[AddressDetail] READONLY
AS

	INSERT INTO [Address].[AddressDetail] (
		[PostcodeId],
		[AddressLine1],
		[AddressLine2],
		[AddressLine3],
		[Locality],
		[LastUpdated]
		)
	SELECT p.Id,
		ad.[AddressLine1],
		ad.[AddressLine2],
		ad.[AddressLine3],
		ad.[Locality],
		ad.[LastUpdated]
	FROM @AddressDetails ad
	INNER JOIN [Address].[Postcode] p 
	ON ad.Postcode = p.Postcode
	LEFT JOIN [Address].[AddressDetail] ad2
	ON p.Id = ad2.PostcodeId
	WHERE ad2.PostcodeId IS NULL  -- only insert addresses for a postcode that hasn't had addresses saved for it yet
  ");


				}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
DROP PROC [Address].[SaveAddresses] 
  ");
						migrationBuilder.Sql(@"
DROP TYPE [Address].[AddressDetail]
  ");




						migrationBuilder.Sql(@"
CREATE TYPE [Address].[AddressDetail] AS TABLE(
	[AddressLine1] [varchar](100) NULL,
	[AddressLine2] [varchar](100) NULL,
	[AddressLine3] [varchar](100) NULL,
	[Locality] [varchar](100) NULL,
	[Postcode] [varchar](10) NOT NULL
)

  ");

            migrationBuilder.Sql(@"
CREATE PROC [Address].[SavePostcodesAndAddresses] 
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

						migrationBuilder.Sql(@"
DROP PROC [Staging].[LoadFromStagingTableAndSwitch]
DROP TABLE [Staging].[Postcode_Switch] 
DROP TABLE [Staging].[Postcode_Staging] 

EXEC ('DROP SCHEMA [Staging]');

  ");



            migrationBuilder.Sql(@"
DROP INDEX [IX_Coordinates] ON [Address].[Postcode]

ALTER TABLE [Address].[Postcode] DROP COLUMN [Coordinates]
  ");


            migrationBuilder.DropColumn(
                name: "Latitude",
                schema: "Address",
                table: "Postcode");

            migrationBuilder.DropColumn(
                name: "Longitude",
                schema: "Address",
                table: "Postcode");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                schema: "Address",
                table: "AddressDetail");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdated",
                schema: "Address",
                table: "Postcode",
                type: "datetime2(0)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(0)",
                oldDefaultValueSql: "GetUtcDate()");
        }



    }
}
