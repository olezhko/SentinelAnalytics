using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SentinelAnalytics.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDetails",
                table: "UserDetails");

            migrationBuilder.DropIndex(
                name: "IX_UserDetails_UserId",
                table: "UserDetails");

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeSubscriptionId",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeSubscriptionStatus",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripePriceId",
                table: "PricingPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDetails",
                table: "UserDetails",
                column: "UserId");

            migrationBuilder.UpdateData(
                table: "PricingPlans",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000001"),
                column: "StripePriceId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PricingPlans",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000002"),
                column: "StripePriceId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PricingPlans",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000003"),
                column: "StripePriceId",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDetails",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "StripeSubscriptionId",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "StripeSubscriptionStatus",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "StripePriceId",
                table: "PricingPlans");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDetails",
                table: "UserDetails",
                columns: new[] { "UserId", "PlanId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_UserId",
                table: "UserDetails",
                column: "UserId",
                unique: true);
        }
    }
}
