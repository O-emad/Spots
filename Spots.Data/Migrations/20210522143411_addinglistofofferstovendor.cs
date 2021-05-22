using Microsoft.EntityFrameworkCore.Migrations;

namespace Spots.Data.Migrations
{
    public partial class addinglistofofferstovendor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Offers_VendorId",
                table: "Offers",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Vendors_VendorId",
                table: "Offers",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Vendors_VendorId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_VendorId",
                table: "Offers");
        }
    }
}
