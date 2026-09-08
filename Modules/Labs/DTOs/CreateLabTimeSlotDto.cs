namespace CampusServicePortal.Modules.Labs.DTOs;

public class CreateLabTimeSlotDto
{
    public int LabId { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }
}