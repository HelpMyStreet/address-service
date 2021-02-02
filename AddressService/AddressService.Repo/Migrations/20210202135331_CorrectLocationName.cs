using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class CorrectLocationName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Instructions", "Name", "ShortName" },
                values: new object[] { "{\"Intro\":null,\"Steps\":[{\"Heading\":\"Information\",\"Detail\":\"Please make sure to arrive 15 minutes before the start of your shift and bring clothing appropriate for the weather on the day as you may be asked to spend some time outside during your shift.\"}],\"Close\":null}", "Ruston Sports and Social Club, Lincoln", "Lincoln (Ruston Sports and Social Club)" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Instructions", "Name", "ShortName" },
                values: new object[] { "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", "Rustons Sports and Social Club, Lincoln", "Lincoln (Rustons Sports and Social Club)" });
        }
    }
}
