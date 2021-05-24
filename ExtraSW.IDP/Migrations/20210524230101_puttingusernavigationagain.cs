using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExtraSW.IDP.Migrations
{
    public partial class puttingusernavigationagain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("0881566d-5229-4667-b0fd-5e889c2fa28c"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("4ca78cf6-7bab-4496-a354-4f969ded9333"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("4f220b22-6147-4437-9169-c533b45a3be8"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("7f6fb4bf-4a19-41bd-9950-56016d7f573b"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("8e587152-9fb5-434d-a595-6b741909f7c1"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("8fbec63c-a0aa-474e-b74b-74973566080e"));

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[,]
                {
                    { new Guid("174b326c-dd14-4f7a-b604-1cec48dc440c"), "dd1fc331-e967-43a4-8061-b36debf989b1", "given_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Alice" },
                    { new Guid("923201f4-9953-4937-ae2b-1f467cbf4e0c"), "ff978cb5-369b-40f3-96a4-b9a132fffaa9", "family_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Smith" },
                    { new Guid("4343c5fd-b732-4eed-bc3c-63ee4641d852"), "aed303a8-2f40-4ba1-bada-19fea6a87529", "role", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Admin" },
                    { new Guid("56c59b65-27c8-46ca-86ba-02cb7cf5ce39"), "4020a1a6-51eb-4061-b296-d0a644a8aef7", "given_name", new Guid("96053525-d4a5-47ee-855e-0ea77fa6c55a"), "Will" },
                    { new Guid("b11162f3-6db9-44c6-9d67-06ad965c6030"), "d11a83d1-08c9-4728-b3aa-4295021a53f1", "family_name", new Guid("96053525-d4a5-47ee-855e-0ea77fa6c55a"), "Smith" },
                    { new Guid("1fc6d943-a6d8-4f5e-9ca6-58213216bc8b"), "3ee6d1a4-962c-4261-a59d-1d5aa6567e7f", "role", new Guid("96053525-d4a5-47ee-855e-0ea77fa6c55a"), "Vendor" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                column: "ConcurrencyStamp",
                value: "fe79f8df-cf02-4baa-ae9c-6af2bdb4591c");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("96053525-d4a5-47ee-855e-0ea77fa6c55a"),
                column: "ConcurrencyStamp",
                value: "ec5f55a5-96a5-4ade-b14a-4f753ede31b6");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("174b326c-dd14-4f7a-b604-1cec48dc440c"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("1fc6d943-a6d8-4f5e-9ca6-58213216bc8b"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("4343c5fd-b732-4eed-bc3c-63ee4641d852"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("56c59b65-27c8-46ca-86ba-02cb7cf5ce39"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("923201f4-9953-4937-ae2b-1f467cbf4e0c"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("b11162f3-6db9-44c6-9d67-06ad965c6030"));

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

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                column: "ConcurrencyStamp",
                value: "3e371a01-cc62-4c05-9adc-3d35337ada2f");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("96053525-d4a5-47ee-855e-0ea77fa6c55a"),
                column: "ConcurrencyStamp",
                value: "ccda23df-8d4c-42a9-b33a-b0eae3fa051b");
        }
    }
}
