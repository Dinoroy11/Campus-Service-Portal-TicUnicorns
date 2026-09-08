
using CampusServicePortal_TicUnicorns.Modules.Canteen.Enums;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;

public class MealAbsence
{
    public int MealAbsenceId { get; set; }

    public int MealSubscriptionId { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public string? Reason { get; set; }

    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

    // Pending / Approved / Rejected / Applied
    public AbsenceStatus Status { get; set; }

    // Number of eligible days calculated
    // for carry-forward/adjustment.
    public int EligibleDays { get; set; }

    public MealSubscription? MealSubscription { get; set; }
}

