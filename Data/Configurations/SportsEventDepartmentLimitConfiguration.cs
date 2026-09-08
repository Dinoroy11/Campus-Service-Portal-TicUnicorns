using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class SportsEventDepartmentLimitConfiguration
    : IEntityTypeConfiguration<SportsEventDepartmentLimit>
{
    public void Configure(
        EntityTypeBuilder<SportsEventDepartmentLimit> builder)
    {
        builder.ToTable("SportsEventDepartmentLimits");

        builder.HasKey(x => x.SportsEventDepartmentLimitId);

        builder.Property(x => x.SportsEventId)
            .IsRequired();

        builder.Property(x => x.DepartmentId)
            .IsRequired();

        builder.Property(x => x.RegistrationLimit)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.SportsEventId,
            x.DepartmentId
        })
        .IsUnique();

        builder.HasOne(x => x.SportsEvent)
            .WithMany(x => x.DepartmentLimits)
            .HasForeignKey(x => x.SportsEventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}