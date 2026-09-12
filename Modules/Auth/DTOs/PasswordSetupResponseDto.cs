namespace CampusServicePortal_TicUnicorns.Modules.Auth.DTOs;

public class PasswordSetupResponseDto
{
    public string SetupToken { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public string Username { get; set; } = string.Empty;
}