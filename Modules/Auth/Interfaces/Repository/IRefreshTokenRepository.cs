using CampusServicePortal.Modules.Auth.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Repository;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);

    Task AddAsync(RefreshToken refreshToken);

    Task UpdateAsync(RefreshToken refreshToken);

    Task RevokeAllAsync(int userId);

    Task RevokeActiveAsync(int userId);
}

