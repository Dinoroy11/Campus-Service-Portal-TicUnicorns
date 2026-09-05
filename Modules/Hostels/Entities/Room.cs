namespace CampusServicePortal.Modules.Hostels.Entities;

public class Room
{
    public int RoomId { get; set; }

    public int FloorId { get; set; }

    public string RoomNumber { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public string RoomType { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public Floor Floor { get; set; } = null!;

    public ICollection<RoomBed> RoomBeds { get; set; } = new List<RoomBed>();
}