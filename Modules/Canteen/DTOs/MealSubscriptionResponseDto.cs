
using CampusServicePortal_TicUnicorns.Modules.Canteen.Enums;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;

public class MealSubscriptionResponseDto
{
    public int MealSubscriptionId { get; set; }

    public int StudentId { get; set; }

    public int MealPackageId { get; set; }

    public string PackageCode { get; set; } = string.Empty;

    public string PackageName { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal Amount { get; set; }

    public int? StudentFeeId { get; set; }

    public SubscriptionStatus Status { get; set; }

    public int UnusedDays { get; set; }

    public int CarryForwardDays { get; set; }
}

