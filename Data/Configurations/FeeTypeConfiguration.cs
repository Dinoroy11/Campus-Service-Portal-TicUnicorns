using CampusServicePortal.Modules.Fees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class FeeTypeConfiguration : IEntityTypeConfiguration<FeeType>
{
    public void Configure(EntityTypeBuilder<FeeType> builder)
    {
        builder.ToTable("FeeTypes");

        builder.HasKey(x => x.FeeTypeId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasMany(x => x.StudentFees)
            .WithOne(x => x.FeeType)
            .HasForeignKey(x => x.FeeTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}