namespace CampusServicePortal.Modules.Hostels.Entities;

public class Hostel
{
    public int HostelId { get; set; }

    public int UniversityId { get; set; }

    public string HostelName { get; set; } = string.Empty;

    public string HostelType { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public ICollection<Floor> Floors { get; set; } = new List<Floor>();
}