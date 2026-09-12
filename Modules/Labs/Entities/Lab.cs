namespace CampusServicePortal.Modules.Labs.Entities;

public class Lab
{
    public int LabId { get; set; }

    public string LabName { get; set; } = string.Empty;

    // Supported values: Science, Computer
    public string LabType { get; set; } = "Science";

    public int Capacity { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<LabSeat> LabSeats { get; set; }
        = new List<LabSeat>();

    public ICollection<LabTimeSlot> LabTimeSlots { get; set; }
        = new List<LabTimeSlot>();

    public ICollection<LabBooking> LabBookings { get; set; }
        = new List<LabBooking>();
}
