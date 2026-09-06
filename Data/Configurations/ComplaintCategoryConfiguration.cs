using CampusServicePortal.Modules.Complaints.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class ComplaintCategoryConfiguration
    : IEntityTypeConfiguration<ComplaintCategory>
{
    public void Configure(
        EntityTypeBuilder<ComplaintCategory> builder)
    {
        builder.ToTable("ComplaintCategories");

        builder.HasKey(x => x.ComplaintCategoryId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasMany(x => x.Complaints)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}