namespace CampusServicePortal.Modules.Hostels.DTOs;

public class HostelBlueprintDto
{
    public int HostelId { get; set; }

    public string HostelName { get; set; } = string.Empty;

    public List<FloorBlueprintDto> Floors { get; set; } = new();
}

public class FloorBlueprintDto
{
    public int FloorId { get; set; }

    public int FloorNumber { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<RoomBlueprintDto> Rooms { get; set; } = new();
}

public class RoomBlueprintDto
{
    public int RoomId { get; set; }

    public string RoomNumber { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public string RoomType { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }

    public List<BedBlueprintDto> Beds { get; set; } = new();
}

public class BedBlueprintDto
{
    public int BedId { get; set; }

    public string BedNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }
}