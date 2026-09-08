
namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;

public class MealPackage
{
    public int MealPackageId { get; set; }

    public string PackageCode { get; set; } = string.Empty;

    public string PackageName { get; set; } = string.Empty;

    public bool BreakfastIncluded { get; set; }

    public bool LunchIncluded { get; set; }

    public bool DinnerIncluded { get; set; }

    public decimal MonthlyPrice { get; set; }

    public bool IsActive { get; set; } = true;
}

