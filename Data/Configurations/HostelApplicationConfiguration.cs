using CampusServicePortal.Modules.Hostels.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class HostelApplicationConfiguration : IEntityTypeConfiguration<HostelApplication>
{
    public void Configure(EntityTypeBuilder<HostelApplication> builder)
    {
        builder.ToTable("HostelApplications");
        builder.HasKey(x => x.HostelApplicationId);

        builder.Property(x => x.District).IsRequired();
        builder.Property(x => x.Province).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne<Student>()
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Hostel>()
            .WithMany()
            .HasForeignKey(x => x.HostelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.StudentId);
        builder.HasIndex(x => x.HostelId);
    }
}
