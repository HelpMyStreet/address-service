using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class EditLouthPCNDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "AddressLine1", "AddressLine2", "AddressLine3", "Latitude", "Longitude", "Name", "ShortName" },
                values: new object[] { "Louth County Hospital", "High Holme Road", "Louth", 53.37101m, -0.008582m, "Louth County Hospital", "Louth County Hospital" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "AddressLine1", "AddressLine2", "AddressLine3", "Latitude", "Longitude", "Name", "ShortName" },
                values: new object[] { "High Holme Rd", "Louth", "", 53.371208m, -0.00451m, "Louth Community Hospital", "Louth" });
        }
    }
}
