using CampusServicePortal.Modules.Gym.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class GymSlotConfiguration : IEntityTypeConfiguration<GymSlot>
{
    public void Configure(EntityTypeBuilder<GymSlot> builder)
    {
        builder.ToTable("GymSlots");
        builder.HasKey(x => x.SlotId);

        builder.Property(x => x.FeeAmount)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.Gym)
            .WithMany(x => x.GymSlots)
            .HasForeignKey(x => x.GymId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.GymId,
            x.SlotDate,
            x.StartTime,
            x.EndTime
        });
    }
}
