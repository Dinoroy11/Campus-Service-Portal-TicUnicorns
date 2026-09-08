using CampusServicePortal_TicUnicorns.Modules.Sports.Enums;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;

public class SportsRegistrationDto
{
    public int SportsRegistrationId { get; set; }

    public int SportsEventId { get; set; }

    public int StudentId { get; set; }

    public SportsRegistrationStatus Status { get; set; }

    public DateTime RegisteredAt { get; set; }
}