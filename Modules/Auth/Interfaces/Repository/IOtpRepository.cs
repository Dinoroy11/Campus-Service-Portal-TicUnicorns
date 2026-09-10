using CampusServicePortal.Modules.Auth.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Repository;

public interface IOtpRepository
{
    Task<OtpVerification?> GetLatestAsync(int userId);

    Task AddAsync(OtpVerification otp);

    Task UpdateAsync(OtpVerification otp);
}