namespace CampusServicePortal.Modules.Hostels.Entities;

public class HostelRoomHold
{
    public int HostelRoomHoldId { get; set; }

    public int StudentId { get; set; }

    public int RoomBedId { get; set; }

    public int? ApplicationId { get; set; }

    public DateTime HeldAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public string Status { get; set; } = string.Empty;
}