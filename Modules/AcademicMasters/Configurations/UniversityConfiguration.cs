using CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Configurations;

public class UniversityConfiguration : IEntityTypeConfiguration<University>
{
    public void Configure(EntityTypeBuilder<University> builder)
    {
        builder.ToTable("Universities");

        builder.HasKey(x => x.UniversityId);

        builder.Property(x => x.UniversityCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.UniversityName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasIndex(x => x.UniversityCode)
            .IsUnique();

        builder.HasIndex(x => x.UniversityName)
            .IsUnique();
    }
}
