
using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;

public class CreateMealPackageDto
{
    [Required]
    [MaxLength(20)]
    public string PackageCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string PackageName { get; set; } = string.Empty;

    public bool BreakfastIncluded { get; set; }

    public bool LunchIncluded { get; set; }

    public bool DinnerIncluded { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MonthlyPrice { get; set; }
}

