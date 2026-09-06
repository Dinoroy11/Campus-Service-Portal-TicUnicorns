using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.DTOs;

public class ForgotPasswordRequestDto
{
    [Required]
    public string Username { get; set; } = string.Empty;
}