using CampusServicePortal.Modules.Auth.Entities;
using CampusServicePortal.Modules.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class OtpVerificationConfiguration
    : IEntityTypeConfiguration<OtpVerification>
{
    public void Configure(
        EntityTypeBuilder<OtpVerification> builder)
    {
        builder.ToTable("OtpVerification");

        builder.HasKey(x => x.OtpVerificationId);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.OtpHash)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.VerifiedAt)
            .IsRequired(false);

        builder.Property(x => x.AttemptCount)
            .IsRequired();

        builder.Property(x => x.IsVerified)
            .IsRequired();

        builder.HasIndex(x => x.UserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}