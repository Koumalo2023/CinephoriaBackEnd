using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinephoriaServer.API.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationStatusFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Reservations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "CancellationSent",
                table: "Reservations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInTime",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckOutTime",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ConfirmationSent",
                table: "Reservations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsNoShow",
                table: "Reservations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReminderSent",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NoShowSent",
                table: "Reservations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDueDate",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CancellationSent",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CheckInTime",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CheckOutTime",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ConfirmationSent",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "IsNoShow",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "LastReminderSent",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "NoShowSent",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "PaymentDueDate",
                table: "Reservations");
        }
    }
}
