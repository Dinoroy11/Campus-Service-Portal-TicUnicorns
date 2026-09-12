using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal.Modules.Gym.Entities;

public class GymSlot
{
    [Key]
    public int SlotId { get; set; }

    public int GymId { get; set; }
    public DateTime SlotDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int MaxCapacity { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool RequiresPayment { get; set; }
    public decimal FeeAmount { get; set; }

    public Gym? Gym { get; set; }

    public ICollection<GymBooking> GymBookings { get; set; }
        = new List<GymBooking>();
}
