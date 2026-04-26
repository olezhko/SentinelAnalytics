using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SentinelAnalytics.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPricePlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Plan",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PricingPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxProjects = table.Column<int>(type: "int", nullable: false),
                    MaxEventsPerMonth = table.Column<int>(type: "int", nullable: false),
                    MaxCrashesPerMonth = table.Column<int>(type: "int", nullable: false),
                    MaxTeamMembersPerProject = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDetails",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => new { x.UserId, x.PlanId });
                    table.ForeignKey(
                        name: "FK_UserDetails_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDetails_PricingPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "PricingPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PricingPlans",
                columns: new[] { "Id", "Description", "MaxCrashesPerMonth", "MaxEventsPerMonth", "MaxProjects", "MaxTeamMembersPerProject", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("f0000000-0000-0000-0000-000000000001"), "Perfect for side projects and hobbyists.", 100, 1000, 1, 2, "Free", 0m },
                    { new Guid("f0000000-0000-0000-0000-000000000002"), "For growing teams and production apps.", 5000, 100000, 10, 10, "Pro", 49m },
                    { new Guid("f0000000-0000-0000-0000-000000000003"), "Enterprise-grade scale and support.", 50000, 1000000, 100, 50, "Max", 199m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_PlanId",
                table: "UserDetails",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_UserId",
                table: "UserDetails",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDetails");

            migrationBuilder.DropTable(
                name: "PricingPlans");

            migrationBuilder.DropColumn(
                name: "Plan",
                table: "Projects");
        }
    }
}
