using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class AddLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Location",
                schema: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(unicode: false, maxLength: 200, nullable: false),
                    ShortName = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    AddressLine1 = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    AddressLine2 = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    AddressLine3 = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Locality = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    PostCode = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9, 6)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9, 6)", nullable: false),
                    Instructions = table.Column<string>(unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Location",
                schema: "Address");
        }
    }
}
