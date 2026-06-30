using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppHotelFinal.Migrations
{
    /// <inheritdoc />
    public partial class ViniHog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clients_AppUserId",
                table: "Clients");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "Clients",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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

            migrationBuilder.CreateIndex(
                name: "IX_Clients_AppUserId",
                table: "Clients",
                column: "AppUserId",
                unique: true,
                filter: "[AppUserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clients_AppUserId",
                table: "Clients");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "Clients",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Clients_AppUserId",
                table: "Clients",
                column: "AppUserId",
                unique: true);
        }
    }
}
