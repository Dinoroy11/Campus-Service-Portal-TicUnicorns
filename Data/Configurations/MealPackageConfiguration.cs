using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class MealPackageConfiguration : IEntityTypeConfiguration<MealPackage>
{
    public void Configure(EntityTypeBuilder<MealPackage> builder)
    {
        builder.HasKey(x => x.MealPackageId);

        builder.Property(x => x.PackageCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.PackageCode)
            .IsUnique();

        builder.Property(x => x.PackageName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.MonthlyPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}