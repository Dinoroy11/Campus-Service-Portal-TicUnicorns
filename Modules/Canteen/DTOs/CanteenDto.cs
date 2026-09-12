namespace CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;

public class CanteenDto
{
    public int CanteenId { get; set; }
    public int HostelId { get; set; }
    public string CanteenName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
