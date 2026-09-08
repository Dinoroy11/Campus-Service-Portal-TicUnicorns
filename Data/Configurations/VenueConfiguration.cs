using CampusServicePortal.Modules.Events.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class VenueConfiguration
    : IEntityTypeConfiguration<Venue>
{
    public void Configure(
        EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("Venues");

        builder.HasKey(x => x.VenueId);

        builder.Property(x => x.VenueName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Capacity)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasIndex(x => x.VenueName);
    }
}