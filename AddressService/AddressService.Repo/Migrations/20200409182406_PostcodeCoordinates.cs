using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class PostcodeCoordinates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Delete all data first
            migrationBuilder.Sql(@"
  DELETE FROM [Address].[Postcode]

  TRUNCATE TABLE [Address].[AddressDetail]

  DBCC CHECKIDENT ('[Address].[Postcode]', RESEED, 0);
  ");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                schema: "Address",
                table: "Postcode",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: null);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                schema: "Address",
                table: "Postcode",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: null);


            // Net Core 2.1 doesn't support spacial types :(
            migrationBuilder.Sql(@"
alter table [Address].[Postcode] add [Coordinates] as geography::Point(Latitude, Longitude, 4326) persisted;

create spatial index [IX_Coordinates] on [Address].[Postcode] ([Coordinates])
  ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
