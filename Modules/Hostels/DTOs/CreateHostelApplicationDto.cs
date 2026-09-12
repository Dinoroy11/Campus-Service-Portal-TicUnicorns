namespace CampusServicePortal.Modules.Hostels.DTOs;

public class CreateHostelApplicationDto
{
    public int HostelId { get; set; }
    public int HoldId { get; set; }
    public string District { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string? Reason { get; set; }
}