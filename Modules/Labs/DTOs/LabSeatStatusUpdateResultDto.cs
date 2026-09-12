namespace CampusServicePortal.Modules.Labs.DTOs;

public class LabSeatStatusUpdateResultDto
{
    public int LabSeatId { get; set; }

    public string SeatNumber { get; set; } = string.Empty;

    public string PreviousStatus { get; set; } = string.Empty;

    public string NewStatus { get; set; } = string.Empty;

    public int AffectedBookings { get; set; }

    public int ReassignedBookings { get; set; }

    public int CancelledBookings { get; set; }

    public List<LabSeatBookingImpactDto> BookingImpacts { get; set; }
        = new();
}

public class LabSeatBookingImpactDto
{
    public int LabBookingId { get; set; }

    public int StudentId { get; set; }

    public DateTime BookingDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int OldLabSeatId { get; set; }

    public string OldSeatNumber { get; set; } = string.Empty;

    public int? NewLabSeatId { get; set; }

    public string? NewSeatNumber { get; set; }

    // Reassigned / Cancelled
    public string Action { get; set; } = string.Empty;

    public bool NotificationSent { get; set; }
}