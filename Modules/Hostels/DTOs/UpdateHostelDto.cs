namespace CampusServicePortal.Modules.Hostels.DTOs;

public class UpdateHostelDto
{
    public string HostelName { get; set; } = string.Empty;

    public string HostelType { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}