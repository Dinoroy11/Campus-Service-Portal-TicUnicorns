using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusServicePortal_TicUnicorns.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventSeats_EventId",
                table: "EventSeats");

            migrationBuilder.DropIndex(
                name: "IX_EventSeats_SeatNumber",
                table: "EventSeats");

            migrationBuilder.DropIndex(
                name: "IX_EventRegistrations_EventId",
                table: "EventRegistrations");

            migrationBuilder.CreateTable(
                name: "OtpVerification",
                columns: table => new
                {
                    OtpVerificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OtpHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpVerification", x => x.OtpVerificationId);
                    table.ForeignKey(
                        name: "FK_OtpVerification_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventSeats_EventId_SeatNumber",
                table: "EventSeats",
                columns: new[] { "EventId", "SeatNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_EventId_StudentId",
                table: "EventRegistrations",
                columns: new[] { "EventId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerification_UserId",
                table: "OtpVerification",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventPayments_EventRegistrations_RegistrationId",
                table: "EventPayments",
                column: "RegistrationId",
                principalTable: "EventRegistrations",
                principalColumn: "EventRegistrationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventRegistrations_EventSeats_EventSeatId",
                table: "EventRegistrations",
                column: "EventSeatId",
                principalTable: "EventSeats",
                principalColumn: "EventSeatId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventRegistrations_Events_EventId",
                table: "EventRegistrations",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventRegistrations_Student_StudentId",
                table: "EventRegistrations",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Venues_VenueId",
                table: "Events",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "VenueId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventSeats_Events_EventId",
                table: "EventSeats",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventPayments_EventRegistrations_RegistrationId",
                table: "EventPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_EventRegistrations_EventSeats_EventSeatId",
                table: "EventRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_EventRegistrations_Events_EventId",
                table: "EventRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_EventRegistrations_Student_StudentId",
                table: "EventRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Venues_VenueId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_EventSeats_Events_EventId",
                table: "EventSeats");

            migrationBuilder.DropTable(
                name: "OtpVerification");

            migrationBuilder.DropIndex(
                name: "IX_EventSeats_EventId_SeatNumber",
                table: "EventSeats");

            migrationBuilder.DropIndex(
                name: "IX_EventRegistrations_EventId_StudentId",
                table: "EventRegistrations");

            migrationBuilder.CreateIndex(
                name: "IX_EventSeats_EventId",
                table: "EventSeats",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSeats_SeatNumber",
                table: "EventSeats",
                column: "SeatNumber");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_EventId",
                table: "EventRegistrations",
                column: "EventId");
        }
    }
}
