namespace CampusServicePortal.Modules.Hostels.DTOs;

public class CreateFloorDto
{
    public int HostelId { get; set; }

    public int FloorNumber { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}