using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal.Modules.Gym.DTOs;

public class CreateGymSlotDto
{
    [Range(1, int.MaxValue)]
    public int GymId { get; set; }

    public DateTime SlotDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    [Range(1, 10000)]
    public int MaxCapacity { get; set; }

    public bool RequiresPayment { get; set; }

    [Range(0, 1000000)]
    public decimal FeeAmount { get; set; }
}
