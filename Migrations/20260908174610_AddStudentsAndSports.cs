using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusServicePortal_TicUnicorns.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentsAndSports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SportsEvents",
                columns: table => new
                {
                    SportsEventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportsEvents", x => x.SportsEventId);
                });

            migrationBuilder.CreateTable(
                name: "StudentMasterList",
                columns: table => new
                {
                    MasterStudentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniversityId = table.Column<int>(type: "int", nullable: false),
                    FacultyId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    UniversityStudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StudentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentMasterList", x => x.MasterStudentId);
                });

            migrationBuilder.CreateTable(
                name: "CoachMeetings",
                columns: table => new
                {
                    CoachMeetingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SportsEventId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    MeetingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MeetingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachMeetings", x => x.CoachMeetingId);
                    table.ForeignKey(
                        name: "FK_CoachMeetings_SportsEvents_SportsEventId",
                        column: x => x.SportsEventId,
                        principalTable: "SportsEvents",
                        principalColumn: "SportsEventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoachMeetings_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SportsEventDepartmentLimits",
                columns: table => new
                {
                    SportsEventDepartmentLimitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SportsEventId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    RegistrationLimit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportsEventDepartmentLimits", x => x.SportsEventDepartmentLimitId);
                    table.ForeignKey(
                        name: "FK_SportsEventDepartmentLimits_SportsEvents_SportsEventId",
                        column: x => x.SportsEventId,
                        principalTable: "SportsEvents",
                        principalColumn: "SportsEventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MasterStudentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.StudentId);
                    table.ForeignKey(
                        name: "FK_Student_StudentMasterList_MasterStudentId",
                        column: x => x.MasterStudentId,
                        principalTable: "StudentMasterList",
                        principalColumn: "MasterStudentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Student_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SportsRegistrations",
                columns: table => new
                {
                    SportsRegistrationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SportsEventId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportsRegistrations", x => x.SportsRegistrationId);
                    table.ForeignKey(
                        name: "FK_SportsRegistrations_SportsEvents_SportsEventId",
                        column: x => x.SportsEventId,
                        principalTable: "SportsEvents",
                        principalColumn: "SportsEventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SportsRegistrations_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoachMeetings_CreatedByUserId",
                table: "CoachMeetings",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CoachMeetings_SportsEventId",
                table: "CoachMeetings",
                column: "SportsEventId");

            migrationBuilder.CreateIndex(
                name: "IX_SportsEventDepartmentLimits_SportsEventId_DepartmentId",
                table: "SportsEventDepartmentLimits",
                columns: new[] { "SportsEventId", "DepartmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SportsRegistrations_SportsEventId_StudentId",
                table: "SportsRegistrations",
                columns: new[] { "SportsEventId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SportsRegistrations_StudentId",
                table: "SportsRegistrations",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_MasterStudentId",
                table: "Student",
                column: "MasterStudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Student_UserId",
                table: "Student",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StudentMasterList_UniversityStudentId",
                table: "StudentMasterList",
                column: "UniversityStudentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoachMeetings");

            migrationBuilder.DropTable(
                name: "SportsEventDepartmentLimits");

            migrationBuilder.DropTable(
                name: "SportsRegistrations");

            migrationBuilder.DropTable(
                name: "SportsEvents");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "StudentMasterList");
        }
    }
}
