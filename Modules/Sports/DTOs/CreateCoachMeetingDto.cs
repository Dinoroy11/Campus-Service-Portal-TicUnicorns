namespace CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;

public class CreateCoachMeetingDto
{
    public int SportsEventId { get; set; }
    public DateTime MeetingDate { get; set; }
    public TimeSpan MeetingTime { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}
