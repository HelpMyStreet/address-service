using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class ChangeLocationToNegative : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.InsertData(
                schema: "Address",
                table: "Location",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressLine3", "Instructions", "Latitude", "Locality", "Longitude", "Name", "PostCode", "ShortName" },
                values: new object[,]
                {
                    { -1, "Greetwell Road", "Lincoln", "", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.234482m, "Lincolnshire", -0.51499m, "Lincoln County Hospital", "LN2 5QY", "Lincoln County Hospital" },
                    { -2, "Sibsey Road", "Boston", "", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 52.993149m, "Lincolnshire", -0.00684m, "Pilgrim Hospital, Boston", "PE21 9QS", "Boston (Pilgrim Hospital)" },
                    { -3, "High Holme Rd", "Louth", "", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.371208m, "Lincolnshire", -0.00451m, "Louth Community Hospital", "LN11 0EU", "Louth" },
                    { -4, "Grantham Meres Leisure Centre Table Tennis Club", "Grantham Meres Leisure Centre", "Trent Road", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 52.903179m, "Grantham", -0.66045m, "Table Tennis Club, Grantham", "NG31 7XQ", "Grantham" },
                    { -5, "Cliff Villages Medical Practice", "Mere Rd", "Waddington", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.165936m, "Lincoln", -0.535592m, "Waddington Branch Surgery, South Lincoln", "LN5 9NX", "Lincoln South (Waddington Branch Surgery)" },
                    { -6, "Lakeside Healthcare at Stamford", "Wharf Rd", "Stamford", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 52.650925m, "", -0.477465m, "St Marys Medical Practice, Stamford", "PE9 2DH", "Stamford" },
                    { -7, "Franklin Hall", "Halton Road", "Spilsby", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.1723m, "", 0.099136m, "Franklin Hall, Spilsby", "PE23 5LA", "Spilsby" },
                    { -8, "Sidings Medical Practice", "14 Sleaford Rd", "Boston", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 52.975942m, "", -0.033522m, "Sidings Medical Practice, Boston", "PE21 8EG", "Boston (Sidings Medical Practice)" },
                    { -9, "Ruston Sports & Social Club", "Newark Road", "Lincoln", "{\"Intro\":null,\"Steps\":[{\"Heading\":\"Information\",\"Detail\":\"Please make sure to arrive 15 minutes before the start of your shift and bring clothing appropriate for the weather on the day as you may be asked to spend some time outside during your shift.\"}],\"Close\":null}", 53.196498m, "", -0.574294m, "Ruston Sports and Social Club, Lincoln", "LN6 8RN", "Lincoln (Ruston Sports and Social Club)" },
                    { -10, "Portland Medical Practice", "60 Portland St", "Lincoln", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.22372m, "", -0.539074m, "Portland Medical Practice, Lincoln", "LN5 7LB", "Lincoln (Portland Medical Practice)" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -10);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -9);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -8);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -7);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -6);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -5);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -4);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -3);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.InsertData(
                schema: "Address",
                table: "Location",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressLine3", "Instructions", "Latitude", "Locality", "Longitude", "Name", "PostCode", "ShortName" },
                values: new object[,]
                {
                    { 1, "Greetwell Road", "Lincoln", "", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.234482m, "Lincolnshire", -0.51499m, "Lincoln County Hospital", "LN2 5QY", "Lincoln County Hospital" },
                    { 2, "Sibsey Road", "Boston", "", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 52.993149m, "Lincolnshire", -0.00684m, "Pilgrim Hospital, Boston", "PE21 9QS", "Boston (Pilgrim Hospital)" },
                    { 3, "High Holme Rd", "Louth", "", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.371208m, "Lincolnshire", -0.00451m, "Louth Community Hospital", "LN11 0EU", "Louth" },
                    { 4, "Grantham Meres Leisure Centre Table Tennis Club", "Grantham Meres Leisure Centre", "Trent Road", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 52.903179m, "Grantham", -0.66045m, "Table Tennis Club, Grantham", "NG31 7XQ", "Grantham" },
                    { 5, "Cliff Villages Medical Practice", "Mere Rd", "Waddington", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.165936m, "Lincoln", -0.535592m, "Waddington Branch Surgery, South Lincoln", "LN5 9NX", "Lincoln South (Waddington Branch Surgery)" },
                    { 6, "Lakeside Healthcare at Stamford", "Wharf Rd", "Stamford", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 52.650925m, "", -0.477465m, "St Marys Medical Practice, Stamford", "PE9 2DH", "Stamford" },
                    { 7, "Franklin Hall", "Halton Road", "Spilsby", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.1723m, "", 0.099136m, "Franklin Hall, Spilsby", "PE23 5LA", "Spilsby" },
                    { 8, "Sidings Medical Practice", "14 Sleaford Rd", "Boston", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 52.975942m, "", -0.033522m, "Sidings Medical Practice, Boston", "PE21 8EG", "Boston (Sidings Medical Practice)" },
                    { 9, "Ruston Sports & Social Club", "Newark Road", "Lincoln", "{\"Intro\":null,\"Steps\":[{\"Heading\":\"Information\",\"Detail\":\"Please make sure to arrive 15 minutes before the start of your shift and bring clothing appropriate for the weather on the day as you may be asked to spend some time outside during your shift.\"}],\"Close\":null}", 53.196498m, "", -0.574294m, "Ruston Sports and Social Club, Lincoln", "LN6 8RN", "Lincoln (Ruston Sports and Social Club)" },
                    { 10, "Portland Medical Practice", "60 Portland St", "Lincoln", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.22372m, "", -0.539074m, "Portland Medical Practice, Lincoln", "LN5 7LB", "Lincoln (Portland Medical Practice)" }
                });
        }
    }
}
