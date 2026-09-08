
using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;

public class CreateMealSubscriptionDto
{
    [Required]
    public int StudentId { get; set; }

    [Required]
    public int MealPackageId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }
}
