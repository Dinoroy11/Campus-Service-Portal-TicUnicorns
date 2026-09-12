namespace CampusServicePortal_TicUnicorns.Modules.Auth.DTOs;

public class LoginResponseDto
{
    public int UserId { get; set; }

    public string Username { get; set; }
        = string.Empty;

    public string Role { get; set; }
        = string.Empty;

    public string AccessToken { get; set; }
        = string.Empty;

    public string RefreshToken { get; set; }
        = string.Empty;

    public DateTime AccessTokenExpiresAt { get; set; }

    public bool MustChangePassword { get; set; }

    public bool IsPhoneVerified { get; set; }
}