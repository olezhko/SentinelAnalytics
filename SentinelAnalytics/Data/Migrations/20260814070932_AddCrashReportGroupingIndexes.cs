using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SentinelAnalytics.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCrashReportGroupingIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CrashReports_ProjectId_IsResolved",
                table: "CrashReports",
                columns: new[] { "ProjectId", "IsResolved" });

            migrationBuilder.CreateIndex(
                name: "IX_CrashReports_ProjectId_Timestamp",
                table: "CrashReports",
                columns: new[] { "ProjectId", "Timestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CrashReports_ProjectId_IsResolved",
                table: "CrashReports");

            migrationBuilder.DropIndex(
                name: "IX_CrashReports_ProjectId_Timestamp",
                table: "CrashReports");
        }
    }
}
