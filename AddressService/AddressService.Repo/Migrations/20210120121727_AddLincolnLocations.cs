using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class AddLincolnLocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AddressLine1", "AddressLine2", "Instructions", "Latitude", "Locality", "Longitude", "Name", "PostCode", "ShortName" },
                values: new object[] { "Greetwell Road", "Lincoln", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.234482m, "Lincolnshire", -0.51499m, "Lincoln County Hospital", "LN2 5QY", "Lincoln County Hospital" });

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AddressLine1", "AddressLine2", "AddressLine3", "Instructions", "Latitude", "Locality", "Longitude", "Name", "PostCode", "ShortName" },
                values: new object[] { "Sibsey Road", "Boston", "", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 52.993149m, "Lincolnshire", -0.00684m, "Pilgrim Hospital, Boston", "PE21 9QS", "Pilgrim Hospital, Boston" });

            migrationBuilder.InsertData(
                schema: "Address",
                table: "Location",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressLine3", "Instructions", "Latitude", "Locality", "Longitude", "Name", "PostCode", "ShortName" },
                values: new object[,]
                {
                    { 3, "High Holme Rd", "Louth", "", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.371208m, "Lincolnshire", -0.00451m, "Louth Community Hospital", "LN11 0EU", "Louth Community Hospital" },
                    { 4, "Grantham Meres Leisure Centre Table Tennis Club", "Grantham Meres Leisure Centre", "Trent Road", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 52.903179m, "Grantham", -0.66045m, "Table Tennis Club, Grantham", "NG31 7XQ", "Table Tennis Club, Grantham" },
                    { 5, "Cliff Villages Medical Practice", "Mere Rd", "Waddington", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.165936m, "Lincoln", -0.535592m, "Waddington Branch Surgery, South Lincoln", "LN5 9NX", "Waddington Branch Surgery, South Lincoln" },
                    { 6, "Lakeside Healthcare at Stamford", "Wharf Rd", "Stamford", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 52.650925m, "", -0.477465m, "St Marys Medical Practice, Stamford", "PE9 2DH", "St Marys Medical Practice, Stamford" },
                    { 7, "Franklin Hall", "Halton Road", "Spilsby", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.1723m, "", 0.099136m, "Franklin Hall, Spilsby", "PE23 5LA", "Franklin Hall, Spilsby" },
                    { 8, "Sidings Medical Practice", "14 Sleaford Rd", "Boston", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 52.975942m, "", -0.033522m, "Sidings Medical Practice, Boston", "PE21 8EG", "Sidings Medical Practice, Boston" },
                    { 9, "Ruston Sports & Social Club", "Newark Road", "Lincoln", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.196498m, "", -0.574294m, "Rustons Sports and Social Club, Lincoln", "LN6 8RN", "Rustons Sports and Social Club, Lincoln" },
                    { 10, "Portland Medical Practice", "60 Portland St", "Lincoln", "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}", 53.22372m, "", -0.539074m, "Portland Medical Practice, Lincoln", "LN5 7LB", "Portland Medical Practice, Lincoln" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AddressLine1", "AddressLine2", "Instructions", "Latitude", "Locality", "Longitude", "Name", "PostCode", "ShortName" },
                values: new object[] { "Age UK Lincoln & South Lincolnshire", "36 Park Street", "{\"Intro\":\"Location 1 intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location 1 close\"}", 53.230492m, "Lincoln", -0.54142m, "Location 1", "LN1 1UQ", "Short Location 1" });

            migrationBuilder.UpdateData(
                schema: "Address",
                table: "Location",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AddressLine1", "AddressLine2", "AddressLine3", "Instructions", "Latitude", "Locality", "Longitude", "Name", "PostCode", "ShortName" },
                values: new object[] { "Location 2 Address Line 1", "Location 2 Address Line 2", "Location 2 Address Line 3", "{\"Intro\":\"Location 2 intro\",\"Steps\":[{\"Heading\":\"Heading 3\",\"Detail\":\"Detail 3\"},{\"Heading\":\"Heading 4\",\"Detail\":\"Detail 4\"}],\"Close\":\"Location 2 close\"}", 53.231289m, "Lincoln", -0.54217m, "Location 2", "LN1 1DD", "Short Location 2" });
        }
    }
}
