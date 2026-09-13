using CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(x => x.DepartmentId);

        builder.Property(x => x.FacultyId)
            .IsRequired();

        builder.Property(x => x.DepartmentCode)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.DepartmentName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.FacultyId,
            x.DepartmentCode
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.FacultyId,
            x.DepartmentName
        }).IsUnique();

        builder.HasOne(x => x.Faculty)
            .WithMany(x => x.Departments)
            .HasForeignKey(x => x.FacultyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
