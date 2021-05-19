using Microsoft.EntityFrameworkCore.Migrations;

namespace Spots.Data.Migrations
{
    public partial class adjustingadswithsortorderandname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Ads",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Ads",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Ads");
        }
    }
}
