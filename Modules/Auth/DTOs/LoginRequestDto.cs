using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.DTOs;

public class LoginRequestDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}