namespace CampusServicePortal.Modules.Labs.Entities;

public class LabTimeSlot
{
    public int TimeSlotId { get; set; }

    public int LabId { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public bool IsActive { get; set; } = true;

    public Lab Lab { get; set; } = null!;

    public ICollection<LabBooking> LabBookings { get; set; }
        = new List<LabBooking>();
}