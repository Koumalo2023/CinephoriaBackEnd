using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinephoriaBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddDtoAndUpdateShowtimeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Showtimes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_AppUserId",
                table: "Showtimes",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Showtimes_AspNetUsers_AppUserId",
                table: "Showtimes",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Showtimes_AspNetUsers_AppUserId",
                table: "Showtimes");

            migrationBuilder.DropIndex(
                name: "IX_Showtimes_AppUserId",
                table: "Showtimes");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Showtimes");
        }
    }
}
