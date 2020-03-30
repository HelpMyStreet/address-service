using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class UniquePostcodeIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PostCode_Postcode",
                schema: "Address",
                table: "PostCode",
                column: "Postcode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PostCode_Postcode",
                schema: "Address",
                table: "PostCode");
        }
    }
}
