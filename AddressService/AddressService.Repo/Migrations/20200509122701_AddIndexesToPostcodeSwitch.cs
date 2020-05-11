using Microsoft.EntityFrameworkCore.Migrations;

namespace AddressService.Repo.Migrations
{
    public partial class AddIndexesToPostcodeSwitch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE UNIQUE NONCLUSTERED INDEX [IXF_Postcode_Postcode] 
ON [Staging].[Postcode_Switch]
(
	[Postcode] ASC
)
INCLUDE ([Latitude], [Longitude]) 
WHERE ([IsActive]=(1))
  ");

            migrationBuilder.Sql(@"
CREATE NONCLUSTERED INDEX [IXF_Postcode_Latitude_Longitude] 
ON [Staging].[Postcode_Switch]
(
	[Latitude] ASC,
	[Longitude] ASC
)
INCLUDE ([Postcode]) 
WHERE ([IsActive]=(1))
  ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP INDEX [IXF_Postcode_Postcode] 
ON [Staging].[Postcode_Switch]
  ");

            migrationBuilder.Sql(@"
DROP INDEX [IXF_Postcode_Latitude_Longitude] 
ON [Staging].[Postcode_Switch]
  ");
        }
    }
}
