namespace CampusServicePortal.Modules.Gym.DTOs;

public class GymBookingDto
{
    public int BookingId { get; set; }
    public int SlotId { get; set; }
    public int UserId { get; set; }
    public int GymId { get; set; }
    public string GymName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool RequiresPayment { get; set; }
    public decimal FeeAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string? PaymentReference { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? HeldAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
