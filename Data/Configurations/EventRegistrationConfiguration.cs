using CampusServicePortal.Modules.Events.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class EventRegistrationConfiguration
    : IEntityTypeConfiguration<EventRegistration>
{
    public void Configure(
        EntityTypeBuilder<EventRegistration> builder)
    {
        builder.ToTable("EventRegistrations");

        builder.HasKey(x => x.EventRegistrationId);

        builder.Property(x => x.EventId)
            .IsRequired();

        builder.Property(x => x.StudentId)
            .IsRequired();

        builder.Property(x => x.EventSeatId)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.HeldAt);

        builder.Property(x => x.ExpiresAt);

        builder.Property(x => x.RegisteredAt)
            .IsRequired();

        builder.HasIndex(x => x.EventId);

        builder.HasIndex(x => x.StudentId);

        builder.HasIndex(x => x.EventSeatId);
    }
}