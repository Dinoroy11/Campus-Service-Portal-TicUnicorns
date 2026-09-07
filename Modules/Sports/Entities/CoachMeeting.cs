namespace CampusServicePortal_TicUnicorns.Modules.Sports.Entities;

public class CoachMeeting
{
    public int CoachMeetingId { get; set; }

    public int SportsEventId { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateTime MeetingDate { get; set; }

    public TimeSpan MeetingTime { get; set; }

    public string? Location { get; set; }

    public string? Notes { get; set; }

    public SportsEvent SportsEvent { get; set; } = null!;
}