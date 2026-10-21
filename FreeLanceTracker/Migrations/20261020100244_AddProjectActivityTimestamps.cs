using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreeLanceTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectActivityTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeEntries_ProjectId",
                table: "TimeEntries");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ClientId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_ClientId",
                table: "Invoices");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_ProjectId_IsBilled_IsBillable",
                table: "TimeEntries",
                columns: new[] { "ProjectId", "IsBilled", "IsBillable" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ClientId_Status",
                table: "Projects",
                columns: new[] { "ClientId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ClientId_Status",
                table: "Invoices",
                columns: new[] { "ClientId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeEntries_ProjectId_IsBilled_IsBillable",
                table: "TimeEntries");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ClientId_Status",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_ClientId_Status",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_ProjectId",
                table: "TimeEntries",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ClientId",
                table: "Projects",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ClientId",
                table: "Invoices",
                column: "ClientId");
        }
    }
}
