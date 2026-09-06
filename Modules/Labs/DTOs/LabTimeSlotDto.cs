namespace CampusServicePortal.Modules.Labs.DTOs;

public class LabTimeSlotDto
{
    public int TimeSlotId { get; set; }

    public int LabId { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public bool IsActive { get; set; }
}