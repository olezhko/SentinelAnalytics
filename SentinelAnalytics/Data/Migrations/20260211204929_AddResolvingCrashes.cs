using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SentinelAnalytics.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddResolvingCrashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsResolved",
                table: "CrashReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ResolutionComment",
                table: "CrashReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "CrashReports",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsResolved",
                table: "CrashReports");

            migrationBuilder.DropColumn(
                name: "ResolutionComment",
                table: "CrashReports");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "CrashReports");
        }
    }
}
