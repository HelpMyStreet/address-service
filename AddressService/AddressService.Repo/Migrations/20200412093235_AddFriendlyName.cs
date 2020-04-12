using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class AddFriendlyName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FriendlyName",
                schema: "Address",
                table: "Postcode",
                unicode: false,
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendlyName",
                schema: "Address",
                table: "Postcode");
        }
    }
}
