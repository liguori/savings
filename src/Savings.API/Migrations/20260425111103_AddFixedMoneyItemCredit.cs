using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Savings.API.Infrastructure;

#nullable disable

namespace Savings.API.Migrations
{
    [DbContext(typeof(SavingsContext))]
    [Migration("20260425111103_AddFixedMoneyItemCredit")]
    public partial class AddFixedMoneyItemCredit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Credit",
                table: "FixedMoneyItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Credit",
                table: "FixedMoneyItems");
        }
    }
}
