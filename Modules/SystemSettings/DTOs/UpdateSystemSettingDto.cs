using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal.Modules.SystemSettings.DTOs;

public class UpdateSystemSettingDto
{
    [Required]
    [StringLength(1000)]
    public string Value { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
}
