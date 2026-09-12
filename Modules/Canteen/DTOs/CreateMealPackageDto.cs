using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;

public class CreateMealPackageDto
{
    [Required]
    public int CanteenId { get; set; }

    // BB / HB / FB
    [Required]
    [MaxLength(10)]
    public string PlanType { get; set; } = string.Empty;

    // Weekly / Monthly
    [Required]
    [MaxLength(20)]
    public string BillingPeriod { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
}
