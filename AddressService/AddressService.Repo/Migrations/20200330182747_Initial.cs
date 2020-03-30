using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Address");

            migrationBuilder.CreateTable(
                name: "PostCode",
                schema: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Postcode = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AddressDetails",
                schema: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PostCodeId = table.Column<int>(nullable: false),
                    AddressLine1 = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    AddressLine2 = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    AddressLine3 = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Locality = table.Column<string>(unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddressDetails_Address_PostCode",
                        column: x => x.PostCodeId,
                        principalSchema: "Address",
                        principalTable: "PostCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddressDetails_PostCodeId",
                schema: "Address",
                table: "AddressDetails",
                column: "PostCodeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressDetails",
                schema: "Address");

            migrationBuilder.DropTable(
                name: "PostCode",
                schema: "Address");
        }
    }
}
