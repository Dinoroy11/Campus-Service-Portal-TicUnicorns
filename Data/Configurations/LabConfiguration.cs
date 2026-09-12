using CampusServicePortal.Modules.Labs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal.Modules.Labs.Configurations;

public class LabConfiguration : IEntityTypeConfiguration<Lab>
{
    public void Configure(EntityTypeBuilder<Lab> builder)
    {
        builder.ToTable("Labs");

        builder.HasKey(x => x.LabId);

        builder.Property(x => x.LabName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.LabType)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Science");

        builder.Property(x => x.Capacity)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasIndex(x => x.LabName)
            .IsUnique();
    }
}
