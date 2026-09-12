using CampusServicePortal_TicUnicorns.Modules.Canteen.Enums;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;

public class CanteenMenuItemDto
{
    public int MenuItemId { get; set; }
    public int CanteenId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MealType MealType { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}
