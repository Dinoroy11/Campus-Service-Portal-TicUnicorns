using CampusServicePortal.Modules.Labs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal.Modules.Labs.Configurations;

public class LabBookingConfiguration
    : IEntityTypeConfiguration<LabBooking>
{
    public void Configure(
        EntityTypeBuilder<LabBooking> builder)
    {
        builder.ToTable("LabBookings");

        builder.HasKey(x => x.LabBookingId);

        builder.Property(x => x.BookingDate)
            .IsRequired();

        builder.Property(x => x.StartTime)
            .IsRequired();

        builder.Property(x => x.EndTime)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Lab → LabBookings
        builder.HasOne(x => x.Lab)
            .WithMany(x => x.LabBookings)
            .HasForeignKey(x => x.LabId)
            .OnDelete(DeleteBehavior.Restrict);

        // TimeSlot → LabBookings
        builder.HasOne(x => x.TimeSlot)
            .WithMany(x => x.LabBookings)
            .HasForeignKey(x => x.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        // LabSeat → LabBookings
        // LabSeatId is nullable
        builder.HasOne(x => x.LabSeat)
            .WithMany(x => x.LabBookings)
            .HasForeignKey(x => x.LabSeatId)
            .OnDelete(DeleteBehavior.Restrict);

        // Availability lookup index
        builder.HasIndex(x => new
        {
            x.LabId,
            x.TimeSlotId,
            x.BookingDate
        });

        // Seat availability lookup index
        builder.HasIndex(x => new
        {
            x.LabSeatId,
            x.TimeSlotId,
            x.BookingDate
        });
    }
}