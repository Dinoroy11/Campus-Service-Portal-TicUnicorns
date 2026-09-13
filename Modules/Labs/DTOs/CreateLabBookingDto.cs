using System.Text.Json.Serialization;

namespace CampusServicePortal.Modules.Labs.DTOs;

public class CreateLabBookingDto
{
    public int LabId { get; set; }

    public int TimeSlotId { get; set; }

    // Required for Computer labs. Must belong to the selected lab.
    // Science labs do not use a seat.
    public int? LabSeatId { get; set; }

    // Set by the backend from the logged-in Student JWT.
    [JsonIgnore]
    public int StudentId { get; set; }

    public DateTime BookingDate { get; set; }

    // Computer lab only.
    // Example: 09:00:00
    public TimeSpan? RequestedStartTime { get; set; }

    // Computer lab only. Maximum 4 hours.
    // Fractional values such as 1.5 are supported.
    public double? RequestedHours { get; set; }
}
