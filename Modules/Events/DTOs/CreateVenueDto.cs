namespace CampusServicePortal.Modules.Events.DTOs;

public class CreateVenueDto
{
    public string VenueName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}