namespace CampusServicePortal_TicUnicorns.Modules.Sports.Entities;

public class SportsEvent
{
    public int SportsEventId { get; set; }

    public string EventName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public string? Location { get; set; }

    public bool IsActive { get; set; }

    public ICollection<SportsEventDepartmentLimit> DepartmentLimits { get; set; }
        = new List<SportsEventDepartmentLimit>();

    public ICollection<SportsRegistration> Registrations { get; set; }
        = new List<SportsRegistration>();

    public ICollection<CoachMeeting> CoachMeetings { get; set; }
        = new List<CoachMeeting>();
}