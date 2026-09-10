namespace CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Service;

public interface IOtpService
{
    Task<string> GenerateAsync(int userId);

    Task VerifyAsync(int userId, string otp);

    Task ResendAsync(int userId);
}