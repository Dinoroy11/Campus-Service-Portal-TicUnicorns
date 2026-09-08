
using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;

public class ReportMealAbsenceDto
{
    [Required]
    public int MealSubscriptionId { get; set; }

    [Required]
    public DateTime FromDate { get; set; }

    [Required]
    public DateTime ToDate { get; set; }

    [MaxLength(250)]
    public string? Reason { get; set; }
}
