namespace CampusServicePortal.Modules.Events.Entities;

public class Event
{
    public int EventId { get; set; }
    public int VenueId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public bool IsPaid { get; set; }
    public bool UsesReservedSeating { get; set; }
    public decimal? FeeAmount { get; set; }
    public int HoldDurationMinutes { get; set; }
    public bool IsActive { get; set; }
}