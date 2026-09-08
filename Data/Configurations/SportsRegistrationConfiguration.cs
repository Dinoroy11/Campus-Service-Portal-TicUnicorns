using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class SportsRegistrationConfiguration
    : IEntityTypeConfiguration<SportsRegistration>
{
    public void Configure(
        EntityTypeBuilder<SportsRegistration> builder)
    {
        builder.ToTable("SportsRegistrations");

        builder.HasKey(x => x.SportsRegistrationId);

        builder.Property(x => x.SportsEventId)
            .IsRequired();

        builder.Property(x => x.StudentId)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.RegisteredAt)
            .IsRequired();

        // Prevent duplicate registration
        // for the same student in the same event.
        builder.HasIndex(x => new
        {
            x.SportsEventId,
            x.StudentId
        })
        .IsUnique();

        builder.HasOne(x => x.SportsEvent)
            .WithMany(x => x.Registrations)
            .HasForeignKey(x => x.SportsEventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Student>()
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}