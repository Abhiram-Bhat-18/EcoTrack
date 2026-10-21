using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class AddWasteReportOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "WasteReports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WasteReports_UserId",
                table: "WasteReports",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WasteReports_Users_UserId",
                table: "WasteReports",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WasteReports_Users_UserId",
                table: "WasteReports");

            migrationBuilder.DropIndex(
                name: "IX_WasteReports_UserId",
                table: "WasteReports");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WasteReports");
        }
    }
}
