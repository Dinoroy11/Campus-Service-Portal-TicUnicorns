using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal.Modules.Hostels.Entities;

public class RoomBed
{
    [Key]
    public int BedId { get; set; }

    public int RoomId { get; set; }

    public string BedNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public Room Room { get; set; } = null!;
}