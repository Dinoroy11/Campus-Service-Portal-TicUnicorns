using CampusServicePortal.Modules.Fees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class FeePaymentConfiguration : IEntityTypeConfiguration<FeePayment>
{
    public void Configure(EntityTypeBuilder<FeePayment> builder)
    {
        builder.ToTable("FeePayments");

        builder.HasKey(x => x.FeePaymentId);

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.PaymentStatus)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.PaymentReference)
            .HasMaxLength(200);

        builder.HasOne(x => x.StudentFee)
            .WithMany(x => x.FeePayments)
            .HasForeignKey(x => x.StudentFeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RefundRequest)
            .WithOne(x => x.Payment)
            .HasForeignKey<RefundRequest>(x => x.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}