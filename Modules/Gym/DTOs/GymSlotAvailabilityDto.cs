namespace CampusServicePortal.Modules.Gym.DTOs;

public class GymSlotAvailabilityDto
{
    public int GymId { get; set; }
    public string GymName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public List<GymSlotDto> Slots { get; set; } = new();
}
