using CampusServicePortal_TicUnicorns.Modules.Sports.Enums;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Entities;

public class SportsRegistration
{
    public int SportsRegistrationId { get; set; }

    public int SportsEventId { get; set; }

    public int StudentId { get; set; }

    public SportsRegistrationStatus Status { get; set; }
    = SportsRegistrationStatus.Registered;

    public DateTime RegisteredAt { get; set; }

    public SportsEvent SportsEvent { get; set; } = null!;
}