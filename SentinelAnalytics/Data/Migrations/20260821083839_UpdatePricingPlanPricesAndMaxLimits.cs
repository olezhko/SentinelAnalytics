using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SentinelAnalytics.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePricingPlanPricesAndMaxLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PricingPlans",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000002"),
                column: "Price",
                value: 19m);

            migrationBuilder.UpdateData(
                table: "PricingPlans",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000003"),
                columns: new[] { "MaxCrashesPerMonth", "MaxEventsPerMonth", "MaxProjects", "MaxTeamMembersPerProject", "Price" },
                values: new object[] { 2147483647, 2147483647, 2147483647, 2147483647, 100m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PricingPlans",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000002"),
                column: "Price",
                value: 49m);

            migrationBuilder.UpdateData(
                table: "PricingPlans",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000003"),
                columns: new[] { "MaxCrashesPerMonth", "MaxEventsPerMonth", "MaxProjects", "MaxTeamMembersPerProject", "Price" },
                values: new object[] { 50000, 1000000, 100, 50, 199m });
        }
    }
}
