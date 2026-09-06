using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.DTOs;

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}