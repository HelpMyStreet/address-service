using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class ChangePostcodeProcName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP PROCEDURE [Staging].[LoadFromStagingTableAndSwitch]
  ");

            migrationBuilder.Sql(@"
CREATE PROCEDURE [Staging].[LoadPostcodesFromStagingTableAndSwitch]
AS
BEGIN
	SET XACT_ABORT ON
	SET	NOCOUNT ON

	BEGIN TRY

		DECLARE @dateNow DATETIME2(0) = GetUTCDate()

		TRUNCATE TABLE [Staging].[Postcode_Switch]

		-- make copy of table (the postcode table is not updated by the application so we won't lose any data)
		SET IDENTITY_INSERT [Staging].[Postcode_Switch] ON
		
		INSERT INTO [Staging].[Postcode_Switch] (
			[Id],
			[Postcode],
			[FriendlyName],
			[LastUpdated],
			[Latitude],
			[Longitude],
			[IsActive]
			)
		SELECT [Id],
			[Postcode],
			[FriendlyName],
			[LastUpdated],
			[Latitude],
			[Longitude],
			[IsActive]
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
				OR sw.[IsActive] != st.[IsActive]
				)
			THEN
				UPDATE
				SET sw.[Latitude] = st.[Latitude],
					sw.[Longitude] = st.[Longitude],
					sw.[LastUpdated] = @dateNow,
					sw.[IsActive] = st.[IsActive]
		--note: when not matched don't need to insert friendly name - it won't have been generated yet and should be null
		WHEN NOT MATCHED BY TARGET
			THEN
				INSERT (
					[Postcode],
					[LastUpdated],
					[Latitude],
					[Longitude],
					[IsActive]
					)
				VALUES (
					st.[Postcode],
					@dateNow,
					st.[Latitude],
					st.[Longitude],
					st.[IsActive]
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
DROP TABLE [Staging].[Postcode_Staging]
  ");


            migrationBuilder.EnsureSchema(
                name: "Staging");

            migrationBuilder.CreateTable(
                name: "Postcode_Staging",
                schema: "Staging",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Postcode = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postcode_Staging", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
DROP PROCEDURE [Staging].[LoadPostcodesFromStagingTableAndSwitch]
  ");

            migrationBuilder.Sql(@"
CREATE PROCEDURE [Staging].[LoadFromStagingTableAndSwitch]
AS
BEGIN
	SET XACT_ABORT ON
	SET	NOCOUNT ON

	BEGIN TRY
		TRUNCATE TABLE [Staging].[Postcode_Switch]

		-- make copy of table (the postcode table is not updated by the application so we won't lose any data)
		SET IDENTITY_INSERT [Staging].[Postcode_Switch] ON
		
		INSERT INTO [Staging].[Postcode_Switch] (
			[Id],
			[Postcode],
			[FriendlyName],
			[LastUpdated],
			[Latitude],
			[Longitude],
			[IsActive]
			)
		SELECT [Id],
			[Postcode],
			[FriendlyName],
			[LastUpdated],
			[Latitude],
			[Longitude],
			[IsActive]
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
				OR sw.[IsActive] != st.[IsActive]
				)
			THEN
				UPDATE
				SET sw.[Latitude] = st.[Latitude],
					sw.[Longitude] = st.[Longitude],
					sw.[LastUpdated] = GetUtcDate(),
					sw.[IsActive] = st.[IsActive]
		--note: when not matched don't need to insert friendly name - it won't have been generated yet and should be null
		WHEN NOT MATCHED BY TARGET
			THEN
				INSERT (
					[Postcode],
					[LastUpdated],
					[Latitude],
					[Longitude],
					[IsActive]
					)
				VALUES (
					st.[Postcode],
					GetUtcDate(),
					st.[Latitude],
					st.[Longitude],
					st.[IsActive]
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
        }
    }
}
