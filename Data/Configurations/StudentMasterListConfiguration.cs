using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class StudentMasterListConfiguration
    : IEntityTypeConfiguration<StudentMasterList>
{
    public void Configure(
        EntityTypeBuilder<StudentMasterList> builder)
    {
        builder.ToTable("StudentMasterList");

        builder.HasKey(x => x.MasterStudentId);

        builder.Property(x => x.UniversityId)
            .IsRequired();

        builder.Property(x => x.FacultyId)
            .IsRequired();

        builder.Property(x => x.DepartmentId)
            .IsRequired();

        builder.Property(x => x.UniversityStudentId)
            .IsRequired();

        builder.Property(x => x.StudentName)
            .IsRequired();

        builder.Property(x => x.MobileNumber)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasIndex(x => x.UniversityStudentId)
            .IsUnique();
    }
}