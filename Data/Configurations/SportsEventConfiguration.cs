using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class SportsEventConfiguration : IEntityTypeConfiguration<SportsEvent>
{
    public void Configure(EntityTypeBuilder<SportsEvent> builder)
    {
        builder.ToTable("SportsEvents");

        builder.HasKey(x => x.SportsEventId);

        builder.Property(x => x.EventName)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired(false);

        builder.Property(x => x.EventDate)
            .IsRequired();

        builder.Property(x => x.StartTime)
            .IsRequired();

        builder.Property(x => x.EndTime)
            .IsRequired();

        builder.Property(x => x.Location)
            .IsRequired(false);

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}