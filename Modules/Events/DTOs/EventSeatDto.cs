namespace CampusServicePortal.Modules.Events.DTOs;

public class EventSeatDto
{
    public int EventSeatId { get; set; }
    public int EventId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public int RowNumber { get; set; }
    public int ColumnNumber { get; set; }
    public string Status { get; set; } = string.Empty;
}