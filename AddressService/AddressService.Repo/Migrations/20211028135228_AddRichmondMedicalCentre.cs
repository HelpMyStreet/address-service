using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class AddRichmondMedicalCentre : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Address",
                table: "Location",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressLine3", "Instructions", "Latitude", "Locality", "Longitude", "Name", "PostCode", "ShortName" },
                values: new object[] { -16, "Richmond Medical Centre", "Lincoln Road", "Lincoln", "{\"Intro\":null,\"Steps\":[{\"Heading\":\"Information\",\"Detail\":\"Please make sure to arrive 15 minutes before the start of your shift and bring clothing appropriate for the weather on the day as you may be asked to spend some time outside during your shift.\"}],\"Close\":null}", 53.183432m, "", -0.587134m, "Richmond Medical Centre (Village Site formerly Crossroads Medical Practice)", "LN6 8NH", "Lincoln (Richmond Medical Centre)" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -16);
        }
    }
}
