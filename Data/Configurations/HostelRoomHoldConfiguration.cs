using CampusServicePortal.Modules.Hostels.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class HostelRoomHoldConfiguration : IEntityTypeConfiguration<HostelRoomHold>
{
    public void Configure(EntityTypeBuilder<HostelRoomHold> builder)
    {
        builder.ToTable("HostelRoomHolds");
        builder.HasKey(x => x.HostelRoomHoldId);

        builder.Property(x => x.HeldAt).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        builder.HasOne<Student>()
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<RoomBed>()
            .WithMany()
            .HasForeignKey(x => x.RoomBedId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<HostelApplication>()
            .WithMany()
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasIndex(x => x.StudentId);
        builder.HasIndex(x => x.RoomBedId);
        builder.HasIndex(x => x.ApplicationId);
    }
}
