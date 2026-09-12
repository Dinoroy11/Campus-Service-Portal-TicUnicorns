using CampusServicePortal.Modules.Hostels.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class HostelAllocationConfiguration : IEntityTypeConfiguration<HostelAllocation>
{
    public void Configure(EntityTypeBuilder<HostelAllocation> builder)
    {
        builder.ToTable("HostelAllocations");
        builder.HasKey(x => x.HostelAllocationId);

        builder.Property(x => x.AllocatedAt).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        builder.HasOne<HostelApplication>()
            .WithMany()
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Student>()
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<RoomBed>()
            .WithMany()
            .HasForeignKey(x => x.BedId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ApplicationId);
        builder.HasIndex(x => x.StudentId);
        builder.HasIndex(x => x.BedId);
    }
}
