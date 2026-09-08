using CampusServicePortal.Modules.Events.Enums;

namespace CampusServicePortal.Modules.Events.DTOs;

public class EventRegistrationDto
{
    public int EventRegistrationId { get; set; }

    public int EventId { get; set; }

    public int StudentId { get; set; }

    public int? EventSeatId { get; set; }

    public EventRegistrationStatus Status { get; set; }
        = EventRegistrationStatus.Held;

    public DateTime? HeldAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime RegisteredAt { get; set; }
}