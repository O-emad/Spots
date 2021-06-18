using Microsoft.EntityFrameworkCore.Migrations;

namespace Spots.Data.Migrations
{
    public partial class addingfollowstovendor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Follows_VendorId",
                table: "Follows",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_Vendors_VendorId",
                table: "Follows",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Follows_Vendors_VendorId",
                table: "Follows");

            migrationBuilder.DropIndex(
                name: "IX_Follows_VendorId",
                table: "Follows");
        }
    }
}
