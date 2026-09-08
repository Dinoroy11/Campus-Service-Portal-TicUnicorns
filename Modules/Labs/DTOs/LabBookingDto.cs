namespace CampusServicePortal.Modules.Labs.DTOs;

public class LabBookingDto
{
    public int LabBookingId { get; set; }

    public int LabId { get; set; }

    public int TimeSlotId { get; set; }

    public int? LabSeatId { get; set; }

    public int StudentId { get; set; }

    public DateTime BookingDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}