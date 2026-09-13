using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusServicePortal_TicUnicorns.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicMasterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Universities",
                columns: table => new
                {
                    UniversityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniversityCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UniversityName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universities", x => x.UniversityId);
                });

            migrationBuilder.CreateTable(
                name: "Faculties",
                columns: table => new
                {
                    FacultyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniversityId = table.Column<int>(type: "int", nullable: false),
                    FacultyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FacultyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculties", x => x.FacultyId);
                    table.ForeignKey(
                        name: "FK_Faculties_Universities_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "Universities",
                        principalColumn: "UniversityId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacultyId = table.Column<int>(type: "int", nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                    table.ForeignKey(
                        name: "FK_Departments_Faculties_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Faculties",
                        principalColumn: "FacultyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentMasterList_DepartmentId",
                table: "StudentMasterList",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentMasterList_FacultyId",
                table: "StudentMasterList",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentMasterList_UniversityId",
                table: "StudentMasterList",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_FacultyId_DepartmentCode",
                table: "Departments",
                columns: new[] { "FacultyId", "DepartmentCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_FacultyId_DepartmentName",
                table: "Departments",
                columns: new[] { "FacultyId", "DepartmentName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Faculties_UniversityId_FacultyCode",
                table: "Faculties",
                columns: new[] { "UniversityId", "FacultyCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Faculties_UniversityId_FacultyName",
                table: "Faculties",
                columns: new[] { "UniversityId", "FacultyName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Universities_UniversityCode",
                table: "Universities",
                column: "UniversityCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Universities_UniversityName",
                table: "Universities",
                column: "UniversityName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentMasterList_Departments_DepartmentId",
                table: "StudentMasterList",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentMasterList_Faculties_FacultyId",
                table: "StudentMasterList",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "FacultyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentMasterList_Universities_UniversityId",
                table: "StudentMasterList",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "UniversityId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentMasterList_Departments_DepartmentId",
                table: "StudentMasterList");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentMasterList_Faculties_FacultyId",
                table: "StudentMasterList");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentMasterList_Universities_UniversityId",
                table: "StudentMasterList");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Faculties");

            migrationBuilder.DropTable(
                name: "Universities");

            migrationBuilder.DropIndex(
                name: "IX_StudentMasterList_DepartmentId",
                table: "StudentMasterList");

            migrationBuilder.DropIndex(
                name: "IX_StudentMasterList_FacultyId",
                table: "StudentMasterList");

            migrationBuilder.DropIndex(
                name: "IX_StudentMasterList_UniversityId",
                table: "StudentMasterList");
        }
    }
}
