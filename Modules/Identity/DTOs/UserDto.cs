namespace CampusServicePortal.Modules.Identity.DTOs;

public class UserDto
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsPhoneVerified { get; set; }

    public bool MustChangePassword { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}