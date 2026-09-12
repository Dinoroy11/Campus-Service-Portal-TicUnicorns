using CampusServicePortal.Modules.Gym.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class GymBookingConfiguration : IEntityTypeConfiguration<GymBooking>
{
    public void Configure(EntityTypeBuilder<GymBooking> builder)
    {
        builder.ToTable("GymBookings");
        builder.HasKey(x => x.BookingId);

        builder.Property(x => x.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.PaymentStatus)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.PaymentReference)
            .HasMaxLength(100);

        builder.HasOne(x => x.GymSlot)
            .WithMany(x => x.GymBookings)
            .HasForeignKey(x => x.SlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.SlotId, x.UserId });
    }
}
