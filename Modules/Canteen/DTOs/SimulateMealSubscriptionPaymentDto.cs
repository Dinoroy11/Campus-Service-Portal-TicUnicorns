using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;

public class SimulateMealSubscriptionPaymentDto
{
    [MaxLength(100)]
    public string? PaymentReference { get; set; }
}
