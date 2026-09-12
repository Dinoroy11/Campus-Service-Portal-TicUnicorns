using System.ComponentModel.DataAnnotations;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Enums;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;

public class CreateCanteenMenuItemDto
{
    [Required]
    public int CanteenId { get; set; }

    [Required]
    [MaxLength(120)]
    public string ItemName { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }

    [Required]
    public MealType MealType { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}
