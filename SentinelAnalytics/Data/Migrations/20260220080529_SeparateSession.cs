using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SentinelAnalytics.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeparateSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AppVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OsVersion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeviceModel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"DELETE FROM CrashReports; DELETE FROM MobileEvents;");

            migrationBuilder.DropColumn(
                name: "AppVersion",
                table: "CrashReports");

            migrationBuilder.DropColumn(
                name: "DeviceModel",
                table: "CrashReports");

            migrationBuilder.DropColumn(
                name: "OsVersion",
                table: "CrashReports");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Projects",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionId",
                table: "MobileEvents",
                type: "uniqueidentifier",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionId",
                table: "CrashReports",
                type: "uniqueidentifier",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Country",
                table: "Sessions",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_DeviceId",
                table: "Sessions",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Language",
                table: "Sessions",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ProjectId",
                table: "Sessions",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_CrashReports_Sessions_SessionId",
                table: "CrashReports",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MobileEvents_Sessions_SessionId",
                table: "MobileEvents",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE s
                FROM Sessions s
                INNER JOIN CrashReports cr ON s.Id = cr.SessionId
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_CrashReports_Sessions_SessionId",
                table: "CrashReports");

            migrationBuilder.DropForeignKey(
                name: "FK_MobileEvents_Sessions_SessionId",
                table: "MobileEvents");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Projects",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "MobileEvents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "CrashReports",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "AppVersion",
                table: "CrashReports",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceModel",
                table: "CrashReports",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OsVersion",
                table: "CrashReports",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
