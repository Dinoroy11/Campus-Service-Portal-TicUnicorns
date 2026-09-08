using CampusServicePortal.Modules.Events.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class EventSeatConfiguration
    : IEntityTypeConfiguration<EventSeat>
{
    public void Configure(EntityTypeBuilder<EventSeat> builder)
    {
        builder.ToTable("EventSeats");

        builder.HasKey(x => x.EventSeatId);

        builder.Property(x => x.EventId)
            .IsRequired();

        builder.Property(x => x.SeatNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.RowNumber)
            .IsRequired();

        builder.Property(x => x.ColumnNumber)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne<Event>()
            .WithMany()
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.EventId, x.SeatNumber })
            .IsUnique();
    }
}