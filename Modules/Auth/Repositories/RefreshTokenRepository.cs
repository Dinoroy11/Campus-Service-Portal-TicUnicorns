
using CampusServicePortal.Modules.Auth.Entities;
using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly CampusDbContext _context;

    public RefreshTokenRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash)
    {
        return await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(x =>
                x.TokenHash == tokenHash);
    }

    public async Task AddAsync(
        RefreshToken refreshToken)
    {
        await _context.Set<RefreshToken>()
            .AddAsync(refreshToken);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        RefreshToken refreshToken)
    {
        _context.Set<RefreshToken>()
            .Update(refreshToken);

        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllAsync(int userId)
    {
        var tokens = await _context.Set<RefreshToken>()
            .Where(x =>
                x.UserId == userId &&
                !x.IsRevoked &&
                x.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task RevokeActiveAsync(int userId)
    {
        var token = await _context.Set<RefreshToken>()
            .Where(x =>
                x.UserId == userId &&
                !x.IsRevoked &&
                x.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (token is null)
            return;

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}

