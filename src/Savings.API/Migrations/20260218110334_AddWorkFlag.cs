using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Savings.API.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecurrencyAdjustements_RecurrentMoneyItems_RecurrentMoneyItemID",
                table: "RecurrencyAdjustements");

            migrationBuilder.DropColumn(
                name: "RecurrencyMoneyItemID",
                table: "RecurrencyAdjustements");

            migrationBuilder.RenameColumn(
                name: "Root",
                table: "RecurrentMoneyItems",
                newName: "Work");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "RecurrentMoneyItems",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<bool>(
                name: "DefaultCredit",
                table: "RecurrentMoneyItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<long>(
                name: "RecurrentMoneyItemID",
                table: "RecurrencyAdjustements",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MoneyCategories",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "MoneyCategories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Cash",
                table: "MaterializedMoneyItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "EndPeriodCashCarry",
                table: "MaterializedMoneyItems",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "FixedMoneyItemID",
                table: "MaterializedMoneyItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurrent",
                table: "MaterializedMoneyItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "RecurrentMoneyItemID",
                table: "MaterializedMoneyItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Work",
                table: "MaterializedMoneyItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "FixedMoneyItems",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<bool>(
                name: "Cash",
                table: "FixedMoneyItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ToVerify",
                table: "FixedMoneyItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Work",
                table: "FixedMoneyItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "CashWithdrawalCategoryID",
                table: "Configuration",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

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
                name: "IX_MaterializedMoneySubitems_CategoryID",
                table: "MaterializedMoneySubitems",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterializedMoneySubitems_MaterializedMoneyItemID",
                table: "MaterializedMoneySubitems",
                column: "MaterializedMoneyItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_RecurrencyAdjustements_RecurrentMoneyItems_RecurrentMoneyItemID",
                table: "RecurrencyAdjustements",
                column: "RecurrentMoneyItemID",
                principalTable: "RecurrentMoneyItems",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecurrencyAdjustements_RecurrentMoneyItems_RecurrentMoneyItemID",
                table: "RecurrencyAdjustements");

            migrationBuilder.DropTable(
                name: "MaterializedMoneySubitems");

            migrationBuilder.DeleteData(
                table: "Configuration",
                keyColumn: "ID",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "MaterializedMoneyItems",
                keyColumn: "ID",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "MoneyCategories",
                keyColumn: "ID",
                keyValue: 16L);

            migrationBuilder.DropColumn(
                name: "DefaultCredit",
                table: "RecurrentMoneyItems");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "MoneyCategories");

            migrationBuilder.DropColumn(
                name: "Cash",
                table: "MaterializedMoneyItems");

            migrationBuilder.DropColumn(
                name: "EndPeriodCashCarry",
                table: "MaterializedMoneyItems");

            migrationBuilder.DropColumn(
                name: "FixedMoneyItemID",
                table: "MaterializedMoneyItems");

            migrationBuilder.DropColumn(
                name: "IsRecurrent",
                table: "MaterializedMoneyItems");

            migrationBuilder.DropColumn(
                name: "RecurrentMoneyItemID",
                table: "MaterializedMoneyItems");

            migrationBuilder.DropColumn(
                name: "Work",
                table: "MaterializedMoneyItems");

            migrationBuilder.DropColumn(
                name: "Cash",
                table: "FixedMoneyItems");

            migrationBuilder.DropColumn(
                name: "ToVerify",
                table: "FixedMoneyItems");

            migrationBuilder.DropColumn(
                name: "Work",
                table: "FixedMoneyItems");

            migrationBuilder.DropColumn(
                name: "CashWithdrawalCategoryID",
                table: "Configuration");

            migrationBuilder.RenameColumn(
                name: "Work",
                table: "RecurrentMoneyItems",
                newName: "Root");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "RecurrentMoneyItems",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "RecurrentMoneyItemID",
                table: "RecurrencyAdjustements",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<long>(
                name: "RecurrencyMoneyItemID",
                table: "RecurrencyAdjustements",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "Description",
                table: "MoneyCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "FixedMoneyItems",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RecurrencyAdjustements_RecurrentMoneyItems_RecurrentMoneyItemID",
                table: "RecurrencyAdjustements",
                column: "RecurrentMoneyItemID",
                principalTable: "RecurrentMoneyItems",
                principalColumn: "ID");
        }
    }
}
