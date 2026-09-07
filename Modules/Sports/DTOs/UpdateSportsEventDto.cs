namespace CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;

public class UpdateSportsEventDto
{
    public string EventName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public string? Location { get; set; }

    public bool IsActive { get; set; }
}