using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SentinelAnalytics.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCrashReportIsIgnored : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsIgnored",
                table: "CrashReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CrashReports_ProjectId_IsIgnored",
                table: "CrashReports",
                columns: new[] { "ProjectId", "IsIgnored" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CrashReports_ProjectId_IsIgnored",
                table: "CrashReports");

            migrationBuilder.DropColumn(
                name: "IsIgnored",
                table: "CrashReports");
        }
    }
}
