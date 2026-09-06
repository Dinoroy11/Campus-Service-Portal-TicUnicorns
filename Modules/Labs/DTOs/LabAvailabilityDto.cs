namespace CampusServicePortal.Modules.Labs.DTOs;

public class LabAvailabilityDto
{
    public int LabId { get; set; }

    public string LabName { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public int BookedCount { get; set; }

    public int AvailableCount { get; set; }

    public bool IsFull { get; set; }

    public bool IsSeatBased { get; set; }

    public List<LabSeatAvailabilityDto> Seats { get; set; }
        = new();
}


public class LabSeatAvailabilityDto
{
    public int LabSeatId { get; set; }

    public string SeatNumber { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }

    public string Status { get; set; } = string.Empty;
}