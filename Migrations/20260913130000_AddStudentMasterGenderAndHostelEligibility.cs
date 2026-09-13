using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusServicePortal_TicUnicorns.Migrations;

[DbContext(typeof(CampusDbContext))]
[Migration("20260913130000_AddStudentMasterGenderAndHostelEligibility")]
public partial class AddStudentMasterGenderAndHostelEligibility : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Gender",
            table: "StudentMasterList",
            type: "nvarchar(20)",
            maxLength: 20,
            nullable: true);

        migrationBuilder.Sql(@"
            UPDATE sml
            SET sml.Gender = s.Gender
            FROM StudentMasterList sml
            INNER JOIN Student s ON s.MasterStudentId = sml.MasterStudentId
            WHERE sml.Gender IS NULL
              AND s.Gender IS NOT NULL
              AND LTRIM(RTRIM(s.Gender)) <> '';
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Gender",
            table: "StudentMasterList");
    }
}
