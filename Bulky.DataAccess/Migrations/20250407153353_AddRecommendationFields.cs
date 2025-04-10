using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyBook.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRecommendationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SalesCount",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "SalesCount", "ViewCount" },
                values: new object[] { new DateTime(2025, 4, 7, 18, 33, 51, 579, DateTimeKind.Local).AddTicks(6707), 0, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "SalesCount", "ViewCount" },
                values: new object[] { new DateTime(2025, 4, 7, 18, 33, 51, 579, DateTimeKind.Local).AddTicks(6720), 0, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "SalesCount", "ViewCount" },
                values: new object[] { new DateTime(2025, 4, 7, 18, 33, 51, 579, DateTimeKind.Local).AddTicks(6722), 0, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "SalesCount", "ViewCount" },
                values: new object[] { new DateTime(2025, 4, 7, 18, 33, 51, 579, DateTimeKind.Local).AddTicks(6723), 0, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "SalesCount", "ViewCount" },
                values: new object[] { new DateTime(2025, 4, 7, 18, 33, 51, 579, DateTimeKind.Local).AddTicks(6725), 0, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "SalesCount", "ViewCount" },
                values: new object[] { new DateTime(2025, 4, 7, 18, 33, 51, 579, DateTimeKind.Local).AddTicks(6726), 0, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SalesCount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Products");
        }
    }
}
