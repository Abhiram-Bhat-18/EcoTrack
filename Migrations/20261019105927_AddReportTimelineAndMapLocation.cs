using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class AddReportTimelineAndMapLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CollectedAt",
                table: "WasteReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "WasteReports",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "WasteReports",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "WasteReports",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectedAt",
                table: "WasteReports");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "WasteReports");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "WasteReports");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "WasteReports");
        }
    }
}
