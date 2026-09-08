
using CampusServicePortal_TicUnicorns.Modules.Canteen.Enums;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;

public class MealUsage
{
    public int MealUsageId { get; set; }

    public int MealSubscriptionId { get; set; }

    public int StudentId { get; set; }

    public DateTime MealDate { get; set; }

    // Breakfast / Lunch / Dinner
    public MealType MealType { get; set; }

    // Eligible / Collected / Missed / Cancelled
    public MealUsageStatus Status { get; set; }

    public DateTime? CollectedAt { get; set; }

    public MealSubscription? MealSubscription { get; set; }
}

