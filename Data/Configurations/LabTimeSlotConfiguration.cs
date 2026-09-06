using CampusServicePortal.Modules.Labs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal.Modules.Labs.Configurations;

public class LabTimeSlotConfiguration
    : IEntityTypeConfiguration<LabTimeSlot>
{
    public void Configure(
        EntityTypeBuilder<LabTimeSlot> builder)
    {
        builder.ToTable("LabTimeSlots");

        builder.HasKey(x => x.TimeSlotId);

        builder.Property(x => x.StartTime)
            .IsRequired();

        builder.Property(x => x.EndTime)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasOne(x => x.Lab)
            .WithMany(x => x.LabTimeSlots)
            .HasForeignKey(x => x.LabId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.LabId,
            x.StartTime,
            x.EndTime
        });
    }
}