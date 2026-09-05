namespace CampusServicePortal.Modules.Hostels.Entities;

public class Floor
{
    public int FloorId { get; set; }

    public int HostelId { get; set; }

    public int FloorNumber { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public Hostel Hostel { get; set; } = null!;

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}