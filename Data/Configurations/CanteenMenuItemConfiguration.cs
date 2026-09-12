using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class CanteenMenuItemConfiguration : IEntityTypeConfiguration<CanteenMenuItem>
{
    public void Configure(EntityTypeBuilder<CanteenMenuItem> builder)
    {
        builder.ToTable("CanteenMenu");

        builder.HasKey(x => x.MenuItemId);

        builder.Property(x => x.ItemName)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(x => x.Description)
            .HasMaxLength(300);

        builder.Property(x => x.MealType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.IsAvailable)
            .IsRequired();

        builder.HasOne(x => x.Canteen)
            .WithMany()
            .HasForeignKey(x => x.CanteenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.CanteenId, x.MealType, x.ItemName });
    }
}
