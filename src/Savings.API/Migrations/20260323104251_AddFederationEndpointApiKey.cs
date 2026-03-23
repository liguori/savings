using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savings.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFederationEndpointApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "FederationEndpoints",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "FederationEndpoints");
        }
    }
}
