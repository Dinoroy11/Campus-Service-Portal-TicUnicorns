namespace CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;

public class CanteenEligibilityDto
{
    public bool IsEligible { get; set; }
    public int? HostelId { get; set; }
    public string Message { get; set; } = string.Empty;
}
