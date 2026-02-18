using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Savings.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EndPeriodRecurrencyType = table.Column<int>(type: "INTEGER", nullable: false),
                    EndPeriodRecurrencyInterval = table.Column<short>(type: "INTEGER", nullable: false),
                    CashWithdrawalCategoryID = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MoneyCategories",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Icon = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoneyCategories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FixedMoneyItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: true),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    AccumulateForBudget = table.Column<bool>(type: "INTEGER", nullable: false),
                    Cash = table.Column<bool>(type: "INTEGER", nullable: false),
                    TimelineWeight = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryID = table.Column<long>(type: "INTEGER", nullable: true),
                    ToVerify = table.Column<bool>(type: "INTEGER", nullable: false),
                    Work = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedMoneyItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FixedMoneyItems_MoneyCategories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "MoneyCategories",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "MaterializedMoneyItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryID = table.Column<long>(type: "INTEGER", nullable: true),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    Projection = table.Column<decimal>(type: "TEXT", nullable: false),
                    EndPeriod = table.Column<bool>(type: "INTEGER", nullable: false),
                    TimelineWeight = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRecurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecurrentMoneyItemID = table.Column<long>(type: "INTEGER", nullable: true),
                    FixedMoneyItemID = table.Column<long>(type: "INTEGER", nullable: true),
                    Cash = table.Column<bool>(type: "INTEGER", nullable: false),
                    EndPeriodCashCarry = table.Column<decimal>(type: "TEXT", nullable: false),
                    Work = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterializedMoneyItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterializedMoneyItems_MoneyCategories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "MoneyCategories",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "RecurrentMoneyItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    RecurrencyInterval = table.Column<int>(type: "INTEGER", nullable: false),
                    RecurrencyType = table.Column<int>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryID = table.Column<long>(type: "INTEGER", nullable: true),
                    TimelineWeight = table.Column<int>(type: "INTEGER", nullable: false),
                    DefaultCredit = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecurrentMoneyItemID = table.Column<long>(type: "INTEGER", nullable: true),
                    Work = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurrentMoneyItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RecurrentMoneyItems_MoneyCategories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "MoneyCategories",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_RecurrentMoneyItems_RecurrentMoneyItems_RecurrentMoneyItemID",
                        column: x => x.RecurrentMoneyItemID,
                        principalTable: "RecurrentMoneyItems",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "MaterializedMoneySubitems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    CategoryID = table.Column<long>(type: "INTEGER", nullable: true),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    Work = table.Column<bool>(type: "INTEGER", nullable: false),
                    MaterializedMoneyItemID = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterializedMoneySubitems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterializedMoneySubitems_MaterializedMoneyItems_MaterializedMoneyItemID",
                        column: x => x.MaterializedMoneyItemID,
                        principalTable: "MaterializedMoneyItems",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MaterializedMoneySubitems_MoneyCategories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "MoneyCategories",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "RecurrencyAdjustements",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecurrencyDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RecurrencyNewDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RecurrencyNewAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    RecurrentMoneyItemID = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurrencyAdjustements", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RecurrencyAdjustements_RecurrentMoneyItems_RecurrentMoneyItemID",
                        column: x => x.RecurrentMoneyItemID,
                        principalTable: "RecurrentMoneyItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Configuration",
                columns: new[] { "ID", "CashWithdrawalCategoryID", "EndPeriodRecurrencyInterval", "EndPeriodRecurrencyType" },
                values: new object[] { 1L, 0L, (short)1, 2 });

            migrationBuilder.InsertData(
                table: "MaterializedMoneyItems",
                columns: new[] { "ID", "Amount", "Cash", "CategoryID", "Date", "EndPeriod", "EndPeriodCashCarry", "FixedMoneyItemID", "IsRecurrent", "Note", "Projection", "RecurrentMoneyItemID", "TimelineWeight", "Type", "Work" },
                values: new object[] { 1L, 0m, false, null, new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Local), true, 0m, null, false, null, 0m, null, 0, 0, false });

            migrationBuilder.InsertData(
                table: "MoneyCategories",
                columns: new[] { "ID", "Description", "Icon" },
                values: new object[,]
                {
                    { 1L, "Food", null },
                    { 2L, "Restaurant", null },
                    { 3L, "Home", null },
                    { 4L, "Car", null },
                    { 5L, "Fun", null },
                    { 6L, "Bills", null },
                    { 7L, "Loan", null },
                    { 8L, "Subscriptions", null },
                    { 9L, "Beauty", null },
                    { 10L, "Clothes", null },
                    { 11L, "Vacation", null },
                    { 12L, "Public Transport", null },
                    { 13L, "Health", null },
                    { 14L, "Gift", null },
                    { 15L, "Culture", null },
                    { 16L, "Other", null }
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
                name: "IX_MaterializedMoneySubitems_CategoryID",
                table: "MaterializedMoneySubitems",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterializedMoneySubitems_MaterializedMoneyItemID",
                table: "MaterializedMoneySubitems",
                column: "MaterializedMoneyItemID");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "FixedMoneyItems");

            migrationBuilder.DropTable(
                name: "MaterializedMoneySubitems");

            migrationBuilder.DropTable(
                name: "RecurrencyAdjustements");

            migrationBuilder.DropTable(
                name: "MaterializedMoneyItems");

            migrationBuilder.DropTable(
                name: "RecurrentMoneyItems");

            migrationBuilder.DropTable(
                name: "MoneyCategories");
        }
    }
}
