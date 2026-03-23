using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFederationEndpoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FederationEndpoints",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FederationEndpoints", x => x.ID);
                });

            migrationBuilder.UpdateData(
                table: "MaterializedMoneyItems",
                keyColumn: "ID",
                keyValue: 1L,
                column: "Date",
                value: new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Local));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FederationEndpoints");

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

            migrationBuilder.UpdateData(
                table: "MaterializedMoneyItems",
                keyColumn: "ID",
                keyValue: 1L,
                column: "Date",
                value: new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Local));
        }
    }
}
