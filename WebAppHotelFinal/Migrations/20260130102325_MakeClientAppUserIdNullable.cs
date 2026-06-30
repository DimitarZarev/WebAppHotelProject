using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppHotelFinal.Migrations
{
    /// <inheritdoc />
    public partial class MakeClientAppUserIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ROLE_ADMIN",
                column: "OnCreated",
                value: new DateTime(2026, 1, 30, 10, 23, 24, 567, DateTimeKind.Utc).AddTicks(4151));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ROLE_EMPLOYEE",
                column: "OnCreated",
                value: new DateTime(2026, 1, 30, 10, 23, 24, 567, DateTimeKind.Utc).AddTicks(4195));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ROLE_ADMIN",
                column: "OnCreated",
                value: new DateTime(2026, 1, 30, 10, 11, 16, 529, DateTimeKind.Utc).AddTicks(8659));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ROLE_EMPLOYEE",
                column: "OnCreated",
                value: new DateTime(2026, 1, 30, 10, 11, 16, 529, DateTimeKind.Utc).AddTicks(8710));
        }
    }
}
