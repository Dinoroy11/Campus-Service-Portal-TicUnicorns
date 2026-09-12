using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class MealSubscriptionConfiguration : IEntityTypeConfiguration<MealSubscription>
{
    public void Configure(EntityTypeBuilder<MealSubscription> builder)
    {
        builder.ToTable("MealSubscriptions");

        builder.HasKey(x => x.MealSubscriptionId);

        builder.Property(x => x.StudentId)
            .IsRequired();

        builder.Property(x => x.MealPackageId)
            .IsRequired();

        builder.Property(x => x.StartDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.EndDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.StudentFeeId)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.PaymentStatus)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.PaymentReference)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.PaidAt)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.MealPackage)
            .WithMany()
            .HasForeignKey(x => x.MealPackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.StudentId,
            x.StartDate,
            x.EndDate
        });
    }
}
