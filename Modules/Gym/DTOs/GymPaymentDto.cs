using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal.Modules.Gym.DTOs;

public class GymPaymentDto
{
    [MaxLength(100)]
    public string? PaymentReference { get; set; }
}
