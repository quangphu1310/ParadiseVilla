using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParadiseVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class AddVillaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 25, 21, 23, 3, 883, DateTimeKind.Local).AddTicks(6064));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 25, 21, 23, 3, 883, DateTimeKind.Local).AddTicks(6100));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 25, 21, 23, 3, 883, DateTimeKind.Local).AddTicks(6102));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 25, 21, 23, 3, 883, DateTimeKind.Local).AddTicks(6103));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 25, 21, 23, 3, 883, DateTimeKind.Local).AddTicks(6105));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 23, 11, 12, 18, 10, DateTimeKind.Local).AddTicks(9758));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 23, 11, 12, 18, 10, DateTimeKind.Local).AddTicks(9772));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 23, 11, 12, 18, 10, DateTimeKind.Local).AddTicks(9774));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 23, 11, 12, 18, 10, DateTimeKind.Local).AddTicks(9803));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 23, 11, 12, 18, 10, DateTimeKind.Local).AddTicks(9805));
        }
    }
}
