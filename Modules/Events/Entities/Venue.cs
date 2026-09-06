namespace CampusServicePortal.Modules.Events.Entities;

public class Venue
{
    public int VenueId { get; set; }

    public string VenueName { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}