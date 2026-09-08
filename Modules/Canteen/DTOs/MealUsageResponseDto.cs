
using CampusServicePortal_TicUnicorns.Modules.Canteen.Enums;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;

public class MealUsageResponseDto
{
    public int MealUsageId { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public DateTime MealDate { get; set; }

    public MealType MealType { get; set; }

    public MealUsageStatus Status { get; set; }

    public DateTime? CollectedAt { get; set; }
}

