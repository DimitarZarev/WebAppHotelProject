using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppHotelFinal.Migrations
{
    /// <inheritdoc />
    public partial class AhhhGoofy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ROLE_ADMIN",
                column: "OnCreated",
                value: new DateTime(2026, 1, 30, 10, 8, 10, 343, DateTimeKind.Utc).AddTicks(7105));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ROLE_EMPLOYEE",
                column: "OnCreated",
                value: new DateTime(2026, 1, 30, 10, 8, 10, 343, DateTimeKind.Utc).AddTicks(7153));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ROLE_ADMIN",
                column: "OnCreated",
                value: new DateTime(2026, 1, 17, 16, 10, 52, 979, DateTimeKind.Utc).AddTicks(9149));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ROLE_EMPLOYEE",
                column: "OnCreated",
                value: new DateTime(2026, 1, 17, 16, 10, 52, 979, DateTimeKind.Utc).AddTicks(9192));
        }
    }
}
