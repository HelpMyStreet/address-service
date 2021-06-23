using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class AddMansfieldCVSLocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Address",
                table: "Location",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressLine3", "Instructions", "Latitude", "Locality", "Longitude", "Name", "PostCode", "ShortName" },
                values: new object[,]
                {
                    { -11, "Wickes Site", "134 Chesterfield Rd S", "Mansfield", "{\"Intro\":null,\"Steps\":[{\"Heading\":\"Information\",\"Detail\":\"Please make sure to arrive 15 minutes before the start of your shift and bring clothing appropriate for the weather on the day as you may be asked to spend some time outside during your shift.\"}],\"Close\":null}", 53.1554539m, "", -1.2070261m, "Mansfield (Wickes Site)", "NG19 7AP", "Mansfield (Wickes Site)" },
                    { -12, "Gamston Community Hall", "Ambleside", "Gamston", "{\"Intro\":null,\"Steps\":[{\"Heading\":\"Information\",\"Detail\":\"Please make sure to arrive 15 minutes before the start of your shift and bring clothing appropriate for the weather on the day as you may be asked to spend some time outside during your shift.\"}],\"Close\":null}", 52.9239686m, "Nottingham", -1.1017603m, "Gamston Community Hall", "NG2 6PS", "Gamston Community Hall" },
                    { -13, "Richard Herrod Centre", "Foxhill Road", "Carlton", "{\"Intro\":null,\"Steps\":[{\"Heading\":\"Information\",\"Detail\":\"Please make sure to arrive 15 minutes before the start of your shift and bring clothing appropriate for the weather on the day as you may be asked to spend some time outside during your shift.\"}],\"Close\":null}", 52.970209m, "Nottingham", -1.1022945m, "Richard Herrod Centre", "NG4 1RL", "Richard Herrod Centre" },
                    { -14, "University of Nottingham King's Meadow Campus", "Lenton Lane", "", "{\"Intro\":null,\"Steps\":[{\"Heading\":\"Information\",\"Detail\":\"Please make sure to arrive 15 minutes before the start of your shift and bring clothing appropriate for the weather on the day as you may be asked to spend some time outside during your shift.\"}],\"Close\":null}", 52.936097m, "Nottingham", -1.1771646m, "King's Meadow Campus", "NG7 2NR", "King's Meadow Campus" },
                    { -15, "Forest Recreation Ground", "Gregory Boulevard", "Forest Fields", "{\"Intro\":null,\"Steps\":[{\"Heading\":\"Information\",\"Detail\":\"Please make sure to arrive 15 minutes before the start of your shift and bring clothing appropriate for the weather on the day as you may be asked to spend some time outside during your shift.\"}],\"Close\":null}", 52.967112m, "Nottingham", -1.1681477m, "Forest Recreation Ground", "NG7 6HB", "Forest Recreation Ground" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -15);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -14);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -13);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -12);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -11);
        }
    }
}
