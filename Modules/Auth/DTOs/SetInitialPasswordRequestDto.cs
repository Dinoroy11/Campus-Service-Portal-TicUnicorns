using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.DTOs;

public class SetInitialPasswordRequestDto
{
    [Required]
    public string SetupToken { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare("NewPassword")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}