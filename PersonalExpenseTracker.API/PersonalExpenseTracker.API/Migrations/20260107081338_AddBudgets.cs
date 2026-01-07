using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExpenseTracker.API.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: new Guid("364717f7-9539-4248-a5e2-12d870885ff8"));

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: new Guid("36b2cfc3-ba1d-4dd2-9950-74ee530e31d7"));

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: new Guid("bf0eaeb8-1147-400e-983b-fdbb6a14642f"));

            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    MonthlyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Expenses",
                columns: new[] { "Id", "Amount", "Category", "Date", "Description" },
                values: new object[,]
                {
                    { new Guid("30143a5c-9578-4e1c-b223-3873abde5e45"), 25.50m, "Food", new DateTime(2026, 1, 2, 8, 13, 36, 60, DateTimeKind.Utc).AddTicks(6309), "Lunch at cafe" },
                    { new Guid("cdb4283c-3817-456a-b649-be14d4b9cf01"), 120.00m, "Bills", new DateTime(2026, 1, 6, 8, 13, 36, 60, DateTimeKind.Utc).AddTicks(6338), "Internet bill" },
                    { new Guid("e224d707-a1f0-448a-8039-548a5059662f"), 50.00m, "Transport", new DateTime(2026, 1, 4, 8, 13, 36, 60, DateTimeKind.Utc).AddTicks(6319), "Gas for car" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_Category_Month_Year",
                table: "Budgets",
                columns: new[] { "Category", "Month", "Year" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: new Guid("30143a5c-9578-4e1c-b223-3873abde5e45"));

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: new Guid("cdb4283c-3817-456a-b649-be14d4b9cf01"));

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: new Guid("e224d707-a1f0-448a-8039-548a5059662f"));

            migrationBuilder.InsertData(
                table: "Expenses",
                columns: new[] { "Id", "Amount", "Category", "Date", "Description" },
                values: new object[,]
                {
                    { new Guid("364717f7-9539-4248-a5e2-12d870885ff8"), 25.50m, "Food", new DateTime(2026, 1, 1, 13, 51, 2, 42, DateTimeKind.Utc).AddTicks(1697), "Lunch at cafe" },
                    { new Guid("36b2cfc3-ba1d-4dd2-9950-74ee530e31d7"), 50.00m, "Transport", new DateTime(2026, 1, 3, 13, 51, 2, 42, DateTimeKind.Utc).AddTicks(1714), "Gas for car" },
                    { new Guid("bf0eaeb8-1147-400e-983b-fdbb6a14642f"), 120.00m, "Bills", new DateTime(2026, 1, 5, 13, 51, 2, 42, DateTimeKind.Utc).AddTicks(1725), "Internet bill" }
                });
        }
    }
}
