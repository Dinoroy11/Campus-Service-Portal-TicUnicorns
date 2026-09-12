namespace CampusServicePortal.Modules.Hostels.DTOs;

public class HostelAllocationDto
{
    public int HostelAllocationId { get; set; }
    public int ApplicationId { get; set; }
    public int StudentId { get; set; }
    public int BedId { get; set; }
    public DateTime AllocatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
