using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExtraSW.IDP.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Active", "ConcurrencyStamp", "Password", "Subject", "UserName" },
                values: new object[] { new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), true, "3e371a01-cc62-4c05-9adc-3d35337ada2f", "password", "d860efca-22d9-47fd-8249-791ba61b07c7", "Admin" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Active", "ConcurrencyStamp", "Password", "Subject", "UserName" },
                values: new object[] { new Guid("96053525-d4a5-47ee-855e-0ea77fa6c55a"), true, "ccda23df-8d4c-42a9-b33a-b0eae3fa051b", "password", "b7539694-97e7-4dfe-84da-b4256e1ff5c7", "Vendor" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[,]
                {
                    { new Guid("8e587152-9fb5-434d-a595-6b741909f7c1"), "4265f648-baef-4202-94a5-0f44f273c0ed", "given-name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Alice" },
                    { new Guid("8fbec63c-a0aa-474e-b74b-74973566080e"), "938477eb-f5a8-4634-8db5-a55b47578179", "family-name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Smith" },
                    { new Guid("4ca78cf6-7bab-4496-a354-4f969ded9333"), "ebfb6a2f-3310-403b-9338-d75db268aa39", "role", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "admin" },
                    { new Guid("7f6fb4bf-4a19-41bd-9950-56016d7f573b"), "565d6573-9a2e-40a9-b2e0-322ded8db931", "given-name", new Guid("96053525-d4a5-47ee-855e-0ea77fa6c55a"), "Will" },
                    { new Guid("4f220b22-6147-4437-9169-c533b45a3be8"), "b58a38b7-f07b-45dd-8d5d-9bc2efc8ed15", "family-name", new Guid("96053525-d4a5-47ee-855e-0ea77fa6c55a"), "Smith" },
                    { new Guid("0881566d-5229-4667-b0fd-5e889c2fa28c"), "e3bc55ac-a790-4bfc-894c-180d005c4c34", "role", new Guid("96053525-d4a5-47ee-855e-0ea77fa6c55a"), "vendor" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Subject",
                table: "Users",
                column: "Subject",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
