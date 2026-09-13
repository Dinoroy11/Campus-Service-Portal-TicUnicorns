using CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Configurations;

public class StudentMasterListAcademicMasterConfiguration
    : IEntityTypeConfiguration<StudentMasterList>
{
    public void Configure(EntityTypeBuilder<StudentMasterList> builder)
    {
        builder.HasOne<University>()
            .WithMany()
            .HasForeignKey(x => x.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Faculty>()
            .WithMany()
            .HasForeignKey(x => x.FacultyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
