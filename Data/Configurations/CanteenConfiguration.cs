using CampusServicePortal.Modules.Hostels.Entities;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using CanteenEntity = CampusServicePortal_TicUnicorns.Modules.Canteen.Entities.Canteen;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class CanteenConfiguration : IEntityTypeConfiguration<CanteenEntity>
{
    public void Configure(EntityTypeBuilder<CanteenEntity> builder)
    {
        builder.ToTable("Canteens");

        builder.HasKey(x => x.CanteenId);

        builder.Property(x => x.CanteenName)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(x => x.Description)
            .HasMaxLength(300);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasOne<Hostel>()
            .WithMany()
            .HasForeignKey(x => x.HostelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.HostelId, x.CanteenName })
            .IsUnique();
    }
}
