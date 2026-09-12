namespace CampusServicePortal.Modules.Labs.DTOs;

public class LabAvailabilityDto
{
    public int LabId { get; set; }

    public string LabName { get; set; } = string.Empty;

    public string LabType { get; set; } = string.Empty;

    public int TimeSlotId { get; set; }

    public DateTime BookingDate { get; set; }

    public TimeSpan SlotStartTime { get; set; }

    public TimeSpan SlotEndTime { get; set; }

    public int Capacity { get; set; }

    public int BookedCount { get; set; }

    public int AvailableCount { get; set; }

    public bool IsFull { get; set; }

    public bool IsSeatBased { get; set; }

    // Used for Computer labs.
    public TimeSpan? RequestedStartTime { get; set; }

    public TimeSpan? RequestedEndTime { get; set; }

    public double? RequestedHours { get; set; }

    public double MaxComputerBookingHours { get; set; } = 4;

    public string Message { get; set; } = string.Empty;

    public List<LabSeatAvailabilityDto> Seats { get; set; }
        = new();
}

public class LabSeatAvailabilityDto
{
    public int LabSeatId { get; set; }

    public string SeatNumber { get; set; } = string.Empty;

    // True when this PC can satisfy the requested duration
    // at the requested start time.
    public bool IsAvailable { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool CanBookRequestedDuration { get; set; }

    // Continuous availability from the requested start time.
    public double AvailableHoursFromRequestedStart { get; set; }

    // If the exact request is unavailable, these fields provide
    // a practical alternative on the same day/time-slot.
    public TimeSpan? SuggestedStartTime { get; set; }

    public TimeSpan? SuggestedEndTime { get; set; }

    public double SuggestedHours { get; set; }
}
