using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class MealPackageConfiguration : IEntityTypeConfiguration<MealPackage>
{
    public void Configure(EntityTypeBuilder<MealPackage> builder)
    {
        builder.ToTable("MealPackages");

        builder.HasKey(x => x.MealPackageId);

        builder.Property(x => x.CanteenId)
            .IsRequired(false);

        builder.Property(x => x.PackageCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.PackageCode)
            .IsUnique();

        builder.Property(x => x.PackageName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PlanType)
            .HasMaxLength(10)
            .IsRequired(false);

        builder.Property(x => x.BillingPeriod)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(x => x.MonthlyPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasOne(x => x.Canteen)
            .WithMany()
            .HasForeignKey(x => x.CanteenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.CanteenId,
            x.PlanType,
            x.BillingPeriod
        })
        .IsUnique()
        .HasFilter("[CanteenId] IS NOT NULL AND [PlanType] IS NOT NULL AND [BillingPeriod] IS NOT NULL");
    }
}
