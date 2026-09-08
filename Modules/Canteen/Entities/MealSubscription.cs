

using CampusServicePortal_TicUnicorns.Modules.Canteen.Enums;
namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;

public class MealSubscription
{
    public int MealSubscriptionId { get; set; }

    public int StudentId { get; set; }

    public int MealPackageId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal Amount { get; set; }

    // This will later link the subscription
    // to the Fees module.
    public int? StudentFeeId { get; set; }

    public SubscriptionStatus Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public MealPackage? MealPackage { get; set; }
}

