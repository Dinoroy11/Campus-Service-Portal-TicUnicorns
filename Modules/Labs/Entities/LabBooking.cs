namespace CampusServicePortal.Modules.Labs.Entities;
using CampusServicePortal.Modules.Labs.Enums;

public class LabBooking
{
    public int LabBookingId { get; set; }

    public int LabId { get; set; }

    public int TimeSlotId { get; set; }

    public int? LabSeatId { get; set; }

    public int StudentId { get; set; }

    public DateTime BookingDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public LabBookingStatus Status { get; set; }
    = LabBookingStatus.Booked;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Lab Lab { get; set; } = null!;

    public LabTimeSlot TimeSlot { get; set; } = null!;

    public LabSeat? LabSeat { get; set; }
}