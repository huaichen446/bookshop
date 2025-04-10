using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyBook.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddingRecommendationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductRelations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId1 = table.Column<int>(type: "int", nullable: false),
                    ProductId2 = table.Column<int>(type: "int", nullable: false),
                    RelationScore = table.Column<double>(type: "float", nullable: false),
                    Product1Id = table.Column<int>(type: "int", nullable: false),
                    Product2Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductRelations_Products_Product1Id",
                        column: x => x.Product1Id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ProductRelations_Products_Product2Id",
                        column: x => x.Product2Id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "UserActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ActivityType = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActivities_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 4, 8, 14, 26, 28, 433, DateTimeKind.Local).AddTicks(174));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 4, 8, 14, 26, 28, 433, DateTimeKind.Local).AddTicks(186));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 4, 8, 14, 26, 28, 433, DateTimeKind.Local).AddTicks(188));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 4, 8, 14, 26, 28, 433, DateTimeKind.Local).AddTicks(189));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 4, 8, 14, 26, 28, 433, DateTimeKind.Local).AddTicks(190));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 4, 8, 14, 26, 28, 433, DateTimeKind.Local).AddTicks(191));

            migrationBuilder.CreateIndex(
                name: "IX_ProductRelations_Product1Id",
                table: "ProductRelations",
                column: "Product1Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRelations_Product2Id",
                table: "ProductRelations",
                column: "Product2Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_ProductId",
                table: "UserActivities",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductRelations");

            migrationBuilder.DropTable(
                name: "UserActivities");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 4, 7, 18, 33, 51, 579, DateTimeKind.Local).AddTicks(6707));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 4, 7, 18, 33, 51, 579, DateTimeKind.Local).AddTicks(6720));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 4, 7, 18, 33, 51, 579, DateTimeKind.Local).AddTicks(6722));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 4, 7, 18, 33, 51, 579, DateTimeKind.Local).AddTicks(6723));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 4, 7, 18, 33, 51, 579, DateTimeKind.Local).AddTicks(6725));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 4, 7, 18, 33, 51, 579, DateTimeKind.Local).AddTicks(6726));
        }
    }
}
