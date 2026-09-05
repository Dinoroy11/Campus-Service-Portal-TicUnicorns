namespace CampusServicePortal.Modules.Hostels.DTOs;

public class UpdateFloorDto
{
    public int FloorNumber { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}