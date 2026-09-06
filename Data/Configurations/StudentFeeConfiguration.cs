using CampusServicePortal.Modules.Fees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class StudentFeeConfiguration
    : IEntityTypeConfiguration<StudentFee>
{
    public void Configure(EntityTypeBuilder<StudentFee> builder)
    {
        builder.ToTable("StudentFees");

        builder.HasKey(x => x.StudentFeeId);

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ExamReference)
            .HasMaxLength(200);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.FeeType)
            .WithMany(x => x.StudentFees)
            .HasForeignKey(x => x.FeeTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.FeePayments)
            .WithOne(x => x.StudentFee)
            .HasForeignKey(x => x.StudentFeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}