using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class MealAbsenceConfiguration : IEntityTypeConfiguration<MealAbsence>
{
    public void Configure(EntityTypeBuilder<MealAbsence> builder)
    {
        builder.HasKey(x => x.MealAbsenceId);

        builder.Property(x => x.MealSubscriptionId)
            .IsRequired();

        builder.Property(x => x.FromDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.ToDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasMaxLength(250);

        builder.Property(x => x.ReportedAt)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.EligibleDays)
            .IsRequired();

        builder.HasOne(x => x.MealSubscription)
            .WithMany()
            .HasForeignKey(x => x.MealSubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.MealSubscriptionId,
            x.FromDate,
            x.ToDate
        });
    }
}