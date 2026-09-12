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

    public int? StudentFeeId { get; set; }

    public SubscriptionStatus Status { get; set; }

    // Pending / Paid / Failed / Refunded
    public string PaymentStatus { get; set; } = "Pending";

    public string? PaymentReference { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public MealPackage? MealPackage { get; set; }
}
