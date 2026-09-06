using CampusServicePortal.Modules.Fees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class RefundRequestConfiguration
    : IEntityTypeConfiguration<RefundRequest>
{
    public void Configure(EntityTypeBuilder<RefundRequest> builder)
    {
        builder.ToTable("RefundRequests");

        builder.HasKey(x => x.RefundRequestId);

        builder.Property(x => x.Reason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.RequestedAt)
            .IsRequired();

        builder.HasOne(x => x.Payment)
            .WithOne(x => x.RefundRequest)
            .HasForeignKey<RefundRequest>(x => x.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}