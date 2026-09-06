namespace CampusServicePortal.Modules.Labs.DTOs;

public class CreateLabSeatDto
{
    public int LabId { get; set; }

    public string SeatNumber { get; set; } = string.Empty;
}