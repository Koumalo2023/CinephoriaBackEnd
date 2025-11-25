using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinephoriaServer.API.Migrations
{
    /// <inheritdoc />
    public partial class AddShowtimeStatusFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActualEndTime",
                table: "Showtimes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualStartTime",
                table: "Showtimes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OccupancyRate",
                table: "Showtimes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Showtimes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualEndTime",
                table: "Showtimes");

            migrationBuilder.DropColumn(
                name: "ActualStartTime",
                table: "Showtimes");

            migrationBuilder.DropColumn(
                name: "OccupancyRate",
                table: "Showtimes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Showtimes");
        }
    }
}
