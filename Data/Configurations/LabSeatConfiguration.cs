using CampusServicePortal.Modules.Labs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal.Modules.Labs.Configurations;

public class LabSeatConfiguration
    : IEntityTypeConfiguration<LabSeat>
{
    public void Configure(
        EntityTypeBuilder<LabSeat> builder)
    {
        builder.ToTable("LabSeats");

        builder.HasKey(x => x.LabSeatId);

        builder.Property(x => x.SeatNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.LabId,
            x.SeatNumber
        })
        .IsUnique();

        builder.HasOne(x => x.Lab)
            .WithMany(x => x.LabSeats)
            .HasForeignKey(x => x.LabId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}