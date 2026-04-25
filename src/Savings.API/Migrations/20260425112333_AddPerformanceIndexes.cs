using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Savings.API.Infrastructure;

#nullable disable

namespace Savings.API.Migrations
{
    [DbContext(typeof(SavingsContext))]
    [Migration("20260425112333_AddPerformanceIndexes")]
    public partial class AddPerformanceIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RecurrentMoneyItems_StartDate",
                table: "RecurrentMoneyItems",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_MaterializedMoneyItems_Date",
                table: "MaterializedMoneyItems",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_MaterializedMoneyItems_EndPeriod",
                table: "MaterializedMoneyItems",
                column: "EndPeriod");

            migrationBuilder.CreateIndex(
                name: "IX_FixedMoneyItems_Date",
                table: "FixedMoneyItems",
                column: "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecurrentMoneyItems_StartDate",
                table: "RecurrentMoneyItems");

            migrationBuilder.DropIndex(
                name: "IX_MaterializedMoneyItems_Date",
                table: "MaterializedMoneyItems");

            migrationBuilder.DropIndex(
                name: "IX_MaterializedMoneyItems_EndPeriod",
                table: "MaterializedMoneyItems");

            migrationBuilder.DropIndex(
                name: "IX_FixedMoneyItems_Date",
                table: "FixedMoneyItems");
        }
    }
}
