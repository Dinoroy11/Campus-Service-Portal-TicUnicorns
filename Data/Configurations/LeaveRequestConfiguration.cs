using CampusServicePortal.Modules.Leave.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal.Modules.Leave.Configurations;

public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("LeaveRequests");

        builder.HasKey(x => x.LeaveRequestId);

        builder.Property(x => x.Reason)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(x => new { x.DepartmentId, x.Status });
        builder.HasIndex(x => x.StudentId);

        builder.HasOne(x => x.LeaveType)
            .WithMany(x => x.LeaveRequests)
            .HasForeignKey(x => x.LeaveTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
