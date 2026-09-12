namespace CampusServicePortal.Modules.Hostels.DTOs;

public class HostelApplicationDto
{
    public int HostelApplicationId { get; set; }
    public int StudentId { get; set; }
    public int HostelId { get; set; }
    public int? RequestedBedId { get; set; }
    public string District { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
