using CampusServicePortal.Modules.Events.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class EventPaymentConfiguration
    : IEntityTypeConfiguration<EventPayment>
{
    public void Configure(
        EntityTypeBuilder<EventPayment> builder)
    {
        builder.ToTable("EventPayments");

        builder.HasKey(x => x.EventPaymentId);

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.PaymentStatus)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.PaymentReference)
            .HasMaxLength(200);

        builder.Property(x => x.PaidAt);

        builder.HasIndex(x => x.RegistrationId)
            .IsUnique();
    }
}