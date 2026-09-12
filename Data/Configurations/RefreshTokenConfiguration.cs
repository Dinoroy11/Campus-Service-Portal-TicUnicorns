using CampusServicePortal.Modules.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Configurations;

public class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(
        EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x =>
            x.RefreshTokenId);

        builder.Property(x =>
            x.UserId)
            .IsRequired();

        builder.Property(x =>
            x.TokenHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x =>
            x.ExpiresAt)
            .IsRequired();

        builder.Property(x =>
            x.CreatedAt)
            .IsRequired();

        builder.Property(x =>
            x.RevokedAt)
            .IsRequired(false);

        builder.Property(x =>
            x.IsRevoked)
            .IsRequired();

        builder.HasIndex(x =>
            x.TokenHash)
            .IsUnique();

        builder.HasIndex(x =>
            x.UserId);

        builder.HasOne<CampusServicePortal.Modules.Identity.Entities.User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}