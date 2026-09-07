namespace CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;

public class CoachMeetingDto
{
    public int CoachMeetingId { get; set; }

    public int SportsEventId { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateTime MeetingDate { get; set; }

    public TimeSpan MeetingTime { get; set; }

    public string? Location { get; set; }

    public string? Notes { get; set; }
}