using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal.Modules.Gym.Entities;

public class GymBooking
{
    [Key]
    public int BookingId { get; set; }

    public int SlotId { get; set; }
    public int UserId { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = "Confirmed";
    public string PaymentStatus { get; set; } = "NotRequired";
    public string? PaymentReference { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? HeldAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public GymSlot? GymSlot { get; set; }
}
