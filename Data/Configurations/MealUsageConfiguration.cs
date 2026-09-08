using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class MealUsageConfiguration : IEntityTypeConfiguration<MealUsage>
{
    public void Configure(EntityTypeBuilder<MealUsage> builder)
    {
        builder.HasKey(x => x.MealUsageId);

        builder.Property(x => x.StudentId)
            .IsRequired();

        builder.Property(x => x.MealSubscriptionId)
            .IsRequired();

        builder.Property(x => x.MealDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.MealType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CollectedAt)
            .IsRequired(false);

        builder.HasOne(x => x.MealSubscription)
            .WithMany()
            .HasForeignKey(x => x.MealSubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevent duplicate meal collection
        builder.HasIndex(x => new
        {
            x.MealSubscriptionId,
            x.MealDate,
            x.MealType
        })
        .IsUnique();

        // Faster student meal-history searches
        builder.HasIndex(x => new
        {
            x.StudentId,
            x.MealDate
        });
    }
}