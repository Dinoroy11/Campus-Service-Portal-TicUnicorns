using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;

public class CreateCanteenDto
{
    [Required]
    public int HostelId { get; set; }

    [Required]
    [MaxLength(120)]
    public string CanteenName { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }
}
