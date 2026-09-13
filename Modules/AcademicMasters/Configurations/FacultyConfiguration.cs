using CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Configurations;

public class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
{
    public void Configure(EntityTypeBuilder<Faculty> builder)
    {
        builder.ToTable("Faculties");

        builder.HasKey(x => x.FacultyId);

        builder.Property(x => x.UniversityId)
            .IsRequired();

        builder.Property(x => x.FacultyCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.FacultyName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.UniversityId,
            x.FacultyCode
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.UniversityId,
            x.FacultyName
        }).IsUnique();

        builder.HasOne(x => x.University)
            .WithMany(x => x.Faculties)
            .HasForeignKey(x => x.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
