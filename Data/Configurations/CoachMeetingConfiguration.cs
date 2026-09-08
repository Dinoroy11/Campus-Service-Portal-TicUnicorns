using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class CoachMeetingConfiguration
    : IEntityTypeConfiguration<CoachMeeting>
{
    public void Configure(
        EntityTypeBuilder<CoachMeeting> builder)
    {
        builder.ToTable("CoachMeetings");

        builder.HasKey(x => x.CoachMeetingId);

        builder.Property(x => x.SportsEventId)
            .IsRequired();

        builder.Property(x => x.CreatedByUserId)
            .IsRequired(false);

        builder.Property(x => x.MeetingDate)
            .IsRequired();

        builder.Property(x => x.MeetingTime)
            .IsRequired();

        builder.Property(x => x.Location)
            .IsRequired(false);

        builder.Property(x => x.Notes)
            .IsRequired(false);

        builder.HasOne(x => x.SportsEvent)
            .WithMany(x => x.CoachMeetings)
            .HasForeignKey(x => x.SportsEventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}