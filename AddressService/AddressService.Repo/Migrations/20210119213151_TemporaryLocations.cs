using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class TemporaryLocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Address",
                table: "Location",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressLine3", "Instructions", "Latitude", "Locality", "Longitude", "Name", "PostCode", "ShortName" },
                values: new object[] { 1, "Age UK Lincoln & South Lincolnshire", "36 Park Street", "", "{\"Intro\":\"Location 1 intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location 1 close\"}", 53.230492m, "Lincoln", -0.54142m, "Location 1", "LN1 1UQ", "Short Location 1" });

            migrationBuilder.InsertData(
                schema: "Address",
                table: "Location",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressLine3", "Instructions", "Latitude", "Locality", "Longitude", "Name", "PostCode", "ShortName" },
                values: new object[] { 2, "Location 2 Address Line 1", "Location 2 Address Line 2", "Location 2 Address Line 3", "{\"Intro\":\"Location 2 intro\",\"Steps\":[{\"Heading\":\"Heading 3\",\"Detail\":\"Detail 3\"},{\"Heading\":\"Heading 4\",\"Detail\":\"Detail 4\"}],\"Close\":\"Location 2 close\"}", 53.231289m, "Lincoln", -0.54217m, "Location 2", "LN1 1DD", "Short Location 2" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
