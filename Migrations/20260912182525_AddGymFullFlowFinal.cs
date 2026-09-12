using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusServicePortal_TicUnicorns.Migrations
{
    /// <inheritdoc />
    public partial class AddGymFullFlowFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GymBookings_GymSlots_GymSlotSlotId",
                table: "GymBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_GymSlots_Gyms_GymId",
                table: "GymSlots");

            migrationBuilder.DropIndex(
                name: "IX_GymSlots_GymId",
                table: "GymSlots");

            migrationBuilder.DropIndex(
                name: "IX_GymBookings_GymSlotSlotId",
                table: "GymBookings");

            migrationBuilder.DropColumn(
                name: "GymSlotSlotId",
                table: "GymBookings");

            migrationBuilder.AddColumn<decimal>(
                name: "FeeAmount",
                table: "GymSlots",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresPayment",
                table: "GymSlots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Gyms",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Gyms",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Gyms",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "GymBookings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "GymBookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HeldAt",
                table: "GymBookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "GymBookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "GymBookings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "GymBookings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_GymSlots_GymId_SlotDate_StartTime_EndTime",
                table: "GymSlots",
                columns: new[] { "GymId", "SlotDate", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_GymBookings_SlotId_UserId",
                table: "GymBookings",
                columns: new[] { "SlotId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GymBookings_GymSlots_SlotId",
                table: "GymBookings",
                column: "SlotId",
                principalTable: "GymSlots",
                principalColumn: "SlotId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GymSlots_Gyms_GymId",
                table: "GymSlots",
                column: "GymId",
                principalTable: "Gyms",
                principalColumn: "GymId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GymBookings_GymSlots_SlotId",
                table: "GymBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_GymSlots_Gyms_GymId",
                table: "GymSlots");

            migrationBuilder.DropIndex(
                name: "IX_GymSlots_GymId_SlotDate_StartTime_EndTime",
                table: "GymSlots");

            migrationBuilder.DropIndex(
                name: "IX_GymBookings_SlotId_UserId",
                table: "GymBookings");

            migrationBuilder.DropColumn(
                name: "FeeAmount",
                table: "GymSlots");

            migrationBuilder.DropColumn(
                name: "RequiresPayment",
                table: "GymSlots");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "GymBookings");

            migrationBuilder.DropColumn(
                name: "HeldAt",
                table: "GymBookings");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "GymBookings");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "GymBookings");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "GymBookings");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Gyms",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Gyms",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Gyms",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "GymBookings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<int>(
                name: "GymSlotSlotId",
                table: "GymBookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GymSlots_GymId",
                table: "GymSlots",
                column: "GymId");

            migrationBuilder.CreateIndex(
                name: "IX_GymBookings_GymSlotSlotId",
                table: "GymBookings",
                column: "GymSlotSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_GymBookings_GymSlots_GymSlotSlotId",
                table: "GymBookings",
                column: "GymSlotSlotId",
                principalTable: "GymSlots",
                principalColumn: "SlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_GymSlots_Gyms_GymId",
                table: "GymSlots",
                column: "GymId",
                principalTable: "Gyms",
                principalColumn: "GymId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
