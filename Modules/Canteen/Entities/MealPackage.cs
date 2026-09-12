namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;

public class MealPackage
{
    public int MealPackageId { get; set; }

    // Nullable only to keep any legacy rows readable.
    // All new plans created by the API require a CanteenId.
    public int? CanteenId { get; set; }

    public string PackageCode { get; set; } = string.Empty;

    public string PackageName { get; set; } = string.Empty;

    // BB / HB / FB
    public string PlanType { get; set; } = string.Empty;

    // Weekly / Monthly
    public string BillingPeriod { get; set; } = string.Empty;

    public bool BreakfastIncluded { get; set; }

    public bool LunchIncluded { get; set; }

    public bool DinnerIncluded { get; set; }

    // Legacy database column name is MonthlyPrice.
    // For the final flow this stores the plan price for the selected billing period.
    public decimal MonthlyPrice { get; set; }

    public bool IsActive { get; set; } = true;

    public global::CampusServicePortal_TicUnicorns.Modules.Canteen.Entities.Canteen? Canteen { get; set; }
}
