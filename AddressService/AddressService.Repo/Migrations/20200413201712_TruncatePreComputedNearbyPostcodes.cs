using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class TruncatePreComputedNearbyPostcodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // this to prevent serialisation errors as the JSON property names have changed
            migrationBuilder.Sql(@"
TRUNCATE TABLE [Address].[PreComputedNearestPostcodes]
  ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // this to prevent serialisation errors as the JSON property names have changed
            migrationBuilder.Sql(@"
TRUNCATE TABLE [Address].[PreComputedNearestPostcodes]
  ");
        }
    }
}
