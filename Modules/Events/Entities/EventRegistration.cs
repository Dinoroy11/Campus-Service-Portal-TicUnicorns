namespace CampusServicePortal.Modules.Events.Entities;

public class EventRegistration
{
    public int EventRegistrationId { get; set; }
    public int EventId { get; set; }
    public int StudentId { get; set; }
    public int? EventSeatId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? HeldAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime RegisteredAt { get; set; }
}