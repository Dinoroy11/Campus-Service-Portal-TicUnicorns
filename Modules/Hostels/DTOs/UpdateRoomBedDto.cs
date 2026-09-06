namespace CampusServicePortal.Modules.Hostels.DTOs;

public class UpdateRoomBedDto
{
    public string BedNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}