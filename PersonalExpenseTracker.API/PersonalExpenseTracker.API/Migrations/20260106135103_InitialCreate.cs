using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExpenseTracker.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Expenses",
                columns: new[] { "Id", "Amount", "Category", "Date", "Description" },
                values: new object[,]
                {
                    { new Guid("364717f7-9539-4248-a5e2-12d870885ff8"), 25.50m, "Food", new DateTime(2026, 1, 1, 13, 51, 2, 42, DateTimeKind.Utc).AddTicks(1697), "Lunch at cafe" },
                    { new Guid("36b2cfc3-ba1d-4dd2-9950-74ee530e31d7"), 50.00m, "Transport", new DateTime(2026, 1, 3, 13, 51, 2, 42, DateTimeKind.Utc).AddTicks(1714), "Gas for car" },
                    { new Guid("bf0eaeb8-1147-400e-983b-fdbb6a14642f"), 120.00m, "Bills", new DateTime(2026, 1, 5, 13, 51, 2, 42, DateTimeKind.Utc).AddTicks(1725), "Internet bill" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Category",
                table: "Expenses",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Date",
                table: "Expenses",
                column: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");
        }
    }
}
