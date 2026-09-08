using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal.Modules.Gym.Entities;

public class GymBooking
{
    [Key]
    public int BookingId { get; set; }

    public int SlotId { get; set; }

    public int UserId { get; set; }

    public DateTime BookingDate { get; set; }

    public string Status { get; set; } = "Booked";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public GymSlot? GymSlot { get; set; }
}