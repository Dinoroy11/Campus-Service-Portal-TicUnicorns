namespace CampusServicePortal.Modules.Hostels.DTOs;

public class CreateHostelDto
{
    public int UniversityId { get; set; }

    public string HostelName { get; set; } = string.Empty;

    public string HostelType { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}