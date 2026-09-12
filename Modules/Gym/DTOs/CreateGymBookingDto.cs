using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal.Modules.Gym.DTOs;

public class CreateGymBookingRequestDto
{
    [Range(1, int.MaxValue)]
    public int SlotId { get; set; }
}
