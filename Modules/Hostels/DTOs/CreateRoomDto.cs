namespace CampusServicePortal.Modules.Hostels.DTOs;

public class CreateRoomDto
{
    public int FloorId { get; set; }

    public string RoomNumber { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public string RoomType { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}