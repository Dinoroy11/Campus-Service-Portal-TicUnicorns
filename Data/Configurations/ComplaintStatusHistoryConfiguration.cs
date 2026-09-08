using CampusServicePortal.Modules.Complaints.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class ComplaintStatusHistoryConfiguration
    : IEntityTypeConfiguration<ComplaintStatusHistory>
{
    public void Configure(
        EntityTypeBuilder<ComplaintStatusHistory> builder)
    {
        builder.ToTable("ComplaintStatusHistory");

        builder.HasKey(x => x.ComplaintStatusHistoryId);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Remarks)
            .HasMaxLength(2000);

        builder.Property(x => x.ChangedByUserId)
            .IsRequired();

        builder.Property(x => x.ChangedAt)
            .IsRequired();

        builder.HasOne(x => x.Complaint)
            .WithMany(x => x.StatusHistory)
            .HasForeignKey(x => x.ComplaintId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}