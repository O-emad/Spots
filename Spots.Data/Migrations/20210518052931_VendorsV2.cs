using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Spots.Data.Migrations
{
    public partial class VendorsV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BannerPicFileName",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicFileName",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VendorId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_VendorId",
                table: "Categories",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Vendors_VendorId",
                table: "Categories",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Vendors_VendorId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_VendorId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "BannerPicFileName",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "ProfilePicFileName",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Categories");
        }
    }
}
