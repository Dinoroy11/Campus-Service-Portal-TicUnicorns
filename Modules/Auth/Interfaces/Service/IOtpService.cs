using CampusServicePortal.Modules.Auth.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Service;

public interface IOtpService
{
    Task<OtpVerification> GenerateAsync(int userId);

    Task VerifyAsync(int userId, string otp);

    Task ResendAsync(int userId);
}