using CampusServicePortal.Modules.Leave.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal.Modules.Leave.Configurations;

public class LeaveApprovalConfiguration : IEntityTypeConfiguration<LeaveApproval>
{
    public void Configure(EntityTypeBuilder<LeaveApproval> builder)
    {
        builder.ToTable("LeaveApprovals");

        builder.HasKey(x => x.LeaveApprovalId);

        builder.Property(x => x.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Remarks)
            .HasMaxLength(1000);

        builder.HasOne(x => x.LeaveRequest)
            .WithMany(x => x.Approvals)
            .HasForeignKey(x => x.LeaveRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ApprovedByUser)
            .WithMany()
            .HasForeignKey(x => x.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
