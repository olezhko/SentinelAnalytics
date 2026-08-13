using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SentinelAnalytics.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBillingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingAddress",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingCity",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingCountry",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingState",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingZip",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardBrand",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardExpiry",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardLastFour",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardholderName",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingAddress",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "BillingCity",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "BillingCountry",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "BillingState",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "BillingZip",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "CardBrand",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "CardExpiry",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "CardLastFour",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "CardholderName",
                table: "UserDetails");
        }
    }
}
