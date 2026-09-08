namespace CampusServicePortal.Modules.Hostels.DTOs;

public class CreateRoomBedDto
{
    public int RoomId { get; set; }

    public string BedNumber { get; set; } = string.Empty;

    public string Status { get; set; } = "Available";

    public bool IsActive { get; set; } = true;
}