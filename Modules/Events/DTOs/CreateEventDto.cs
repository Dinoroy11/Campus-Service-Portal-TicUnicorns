namespace CampusServicePortal.Modules.Events.DTOs;

public class CreateEventDto
{
    public int VenueId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public bool IsPaid { get; set; }
    public bool UsesReservedSeating { get; set; }
    public decimal? FeeAmount { get; set; }
    public int HoldDurationMinutes { get; set; }
    public bool IsActive { get; set; } = true;
}