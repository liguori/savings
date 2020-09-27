using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SavingsProjection.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EndPeriodRecurrencyType = table.Column<int>(nullable: false),
                    EndPeriodRecurrencyInterval = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MoneyCategories",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoneyCategories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FixedMoneyItems",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    CategoryID = table.Column<long>(nullable: true),
                    AccumulateForBudget = table.Column<bool>(nullable: false),
                    TimelineWeight = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedMoneyItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FixedMoneyItems_MoneyCategories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "MoneyCategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterializedMoneyItems",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    CategoryID = table.Column<long>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Projection = table.Column<decimal>(nullable: false),
                    EndPeriod = table.Column<bool>(nullable: false),
                    TimelineWeight = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterializedMoneyItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterializedMoneyItems_MoneyCategories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "MoneyCategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecurrentMoneyItems",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    RecurrencyInterval = table.Column<short>(nullable: false),
                    RecurrencyType = table.Column<int>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    CategoryID = table.Column<long>(nullable: true),
                    Root = table.Column<bool>(nullable: false),
                    TimelineWeight = table.Column<short>(nullable: false),
                    RecurrentMoneyItemID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurrentMoneyItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RecurrentMoneyItems_MoneyCategories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "MoneyCategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurrentMoneyItems_RecurrentMoneyItems_RecurrentMoneyItemID",
                        column: x => x.RecurrentMoneyItemID,
                        principalTable: "RecurrentMoneyItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecurrencyAdjustements",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecurrencyMoneyItemID = table.Column<long>(nullable: false),
                    RecurrencyDate = table.Column<DateTime>(nullable: false),
                    RecurrencyNewDate = table.Column<DateTime>(nullable: true),
                    RecurrencyNewAmount = table.Column<decimal>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    RecurrentMoneyItemID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurrencyAdjustements", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RecurrencyAdjustements_RecurrentMoneyItems_RecurrentMoneyItemID",
                        column: x => x.RecurrentMoneyItemID,
                        principalTable: "RecurrentMoneyItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FixedMoneyItems_CategoryID",
                table: "FixedMoneyItems",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterializedMoneyItems_CategoryID",
                table: "MaterializedMoneyItems",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_RecurrencyAdjustements_RecurrentMoneyItemID",
                table: "RecurrencyAdjustements",
                column: "RecurrentMoneyItemID");

            migrationBuilder.CreateIndex(
                name: "IX_RecurrentMoneyItems_CategoryID",
                table: "RecurrentMoneyItems",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_RecurrentMoneyItems_RecurrentMoneyItemID",
                table: "RecurrentMoneyItems",
                column: "RecurrentMoneyItemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "FixedMoneyItems");

            migrationBuilder.DropTable(
                name: "MaterializedMoneyItems");

            migrationBuilder.DropTable(
                name: "RecurrencyAdjustements");

            migrationBuilder.DropTable(
                name: "RecurrentMoneyItems");

            migrationBuilder.DropTable(
                name: "MoneyCategories");
        }
    }
}
