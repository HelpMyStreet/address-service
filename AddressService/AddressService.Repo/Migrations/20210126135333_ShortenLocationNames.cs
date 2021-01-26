using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class ShortenLocationNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 2,
                column: "ShortName",
                value: "Boston (Pilgrim Hospital)");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 3,
                column: "ShortName",
                value: "Louth");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 4,
                column: "ShortName",
                value: "Grantham");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 5,
                column: "ShortName",
                value: "Lincoln South (Waddington Branch Surgery)");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 6,
                column: "ShortName",
                value: "Stamford");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 7,
                column: "ShortName",
                value: "Spilsby");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 8,
                column: "ShortName",
                value: " Boston (Sidings Medical Practice)");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 9,
                column: "ShortName",
                value: "Lincoln (Rustons Sports and Social Club)");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 10,
                column: "ShortName",
                value: "Lincoln (Portland Medical Practice)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 2,
                column: "ShortName",
                value: "Pilgrim Hospital, Boston");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 3,
                column: "ShortName",
                value: "Louth Community Hospital");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 4,
                column: "ShortName",
                value: "Table Tennis Club, Grantham");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 5,
                column: "ShortName",
                value: "Waddington Branch Surgery, South Lincoln");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 6,
                column: "ShortName",
                value: "St Marys Medical Practice, Stamford");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 7,
                column: "ShortName",
                value: "Franklin Hall, Spilsby");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 8,
                column: "ShortName",
                value: "Sidings Medical Practice, Boston");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 9,
                column: "ShortName",
                value: "Rustons Sports and Social Club, Lincoln");

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 10,
                column: "ShortName",
                value: "Portland Medical Practice, Lincoln");
        }
    }
}
