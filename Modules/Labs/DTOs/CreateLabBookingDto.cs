namespace CampusServicePortal.Modules.Labs.DTOs;

public class CreateLabBookingDto
{
    public int LabId { get; set; }

    public int TimeSlotId { get; set; }

    public int? LabSeatId { get; set; }

    public int StudentId { get; set; }

    public DateTime BookingDate { get; set; }
}