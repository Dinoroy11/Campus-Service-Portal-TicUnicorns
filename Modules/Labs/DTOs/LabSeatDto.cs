namespace CampusServicePortal.Modules.Labs.DTOs;

public class LabSeatDto
{
    public int LabSeatId { get; set; }

    public int LabId { get; set; }

    public string SeatNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}