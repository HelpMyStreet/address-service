using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class RenameTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressDetails_Address_PostCode",
                schema: "Address",
                table: "AddressDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostCode",
                schema: "Address",
                table: "PostCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AddressDetails",
                schema: "Address",
                table: "AddressDetails");

            //migrationBuilder.RenameTable(
            //    name: "PostCode",
            //    schema: "Address",
            //    newName: "Postcode",
            //    newSchema: "Address");

            // above code errors with "Either the parameter @objname is ambiguous or the claimed @objtype ((null)) is wrong"
            migrationBuilder.Sql(@"EXEC sp_rename N'[Address].[PostCode]', N'Postcode', 'OBJECT';");

            migrationBuilder.RenameTable(
                name: "AddressDetails",
                schema: "Address",
                newName: "AddressDetail",
                newSchema: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_PostCode_Postcode",
                schema: "Address",
                table: "Postcode",
                newName: "IX_Postcode_Postcode");

            migrationBuilder.RenameColumn(
                name: "PostCodeId",
                schema: "Address",
                table: "AddressDetail",
                newName: "PostcodeId");

            migrationBuilder.RenameIndex(
                name: "IX_AddressDetails_PostCodeId",
                schema: "Address",
                table: "AddressDetail",
                newName: "IX_AddressDetail_PostcodeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Postcode",
                schema: "Address",
                table: "Postcode",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AddressDetail",
                schema: "Address",
                table: "AddressDetail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressDetails_Address_Postcode",
                schema: "Address",
                table: "AddressDetail",
                column: "PostcodeId",
                principalSchema: "Address",
                principalTable: "Postcode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressDetails_Address_Postcode",
                schema: "Address",
                table: "AddressDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Postcode",
                schema: "Address",
                table: "Postcode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AddressDetail",
                schema: "Address",
                table: "AddressDetail");

            migrationBuilder.RenameTable(
                name: "Postcode",
                schema: "Address",
                newName: "PostCode",
                newSchema: "Address");

            migrationBuilder.RenameTable(
                name: "AddressDetail",
                schema: "Address",
                newName: "AddressDetails",
                newSchema: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_Postcode_Postcode",
                schema: "Address",
                table: "PostCode",
                newName: "IX_PostCode_Postcode");

            migrationBuilder.RenameColumn(
                name: "PostcodeId",
                schema: "Address",
                table: "AddressDetails",
                newName: "PostCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_AddressDetail_PostcodeId",
                schema: "Address",
                table: "AddressDetails",
                newName: "IX_AddressDetails_PostCodeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostCode",
                schema: "Address",
                table: "PostCode",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AddressDetails",
                schema: "Address",
                table: "AddressDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressDetails_Address_PostCode",
                schema: "Address",
                table: "AddressDetails",
                column: "PostCodeId",
                principalSchema: "Address",
                principalTable: "PostCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
