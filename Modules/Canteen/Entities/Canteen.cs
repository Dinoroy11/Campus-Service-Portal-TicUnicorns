namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;

public class Canteen
{
    public int CanteenId { get; set; }

    public int HostelId { get; set; }

    public string CanteenName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
