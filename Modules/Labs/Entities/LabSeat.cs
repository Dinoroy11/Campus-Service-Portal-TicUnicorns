using CampusServicePortal.Modules.Labs.Enums;

namespace CampusServicePortal.Modules.Labs.Entities;

public class LabSeat
{
    public int LabSeatId { get; set; }

    public int LabId { get; set; }

    public string SeatNumber { get; set; } = string.Empty;

    public LabSeatStatus Status { get; set; }
       = LabSeatStatus.Available;

    public bool IsActive { get; set; } = true;

    public Lab Lab { get; set; } = null!;

    public ICollection<LabBooking> LabBookings { get; set; }
        = new List<LabBooking>();
}