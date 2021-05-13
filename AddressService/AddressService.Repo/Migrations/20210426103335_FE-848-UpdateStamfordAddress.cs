using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class FE848UpdateStamfordAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -6,
                columns: new[] { "AddressLine1", "AddressLine2", "Locality", "Name" },
                values: new object[] { "St. Mary’s Medical Centre", "Wharf Road", "Lincolnshire", "St. Mary’s Medical Centre, Stamford" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -6,
                columns: new[] { "AddressLine1", "AddressLine2", "Locality", "Name" },
                values: new object[] { "Lakeside Healthcare at Stamford", "Wharf Rd", "", "St Marys Medical Practice, Stamford" });
        }
    }
}
