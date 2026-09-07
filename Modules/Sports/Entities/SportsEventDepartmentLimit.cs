namespace CampusServicePortal_TicUnicorns.Modules.Sports.Entities;

public class SportsEventDepartmentLimit
{
    public int SportsEventDepartmentLimitId { get; set; }

    public int SportsEventId { get; set; }

    public int DepartmentId { get; set; }

    public int RegistrationLimit { get; set; }

    public SportsEvent SportsEvent { get; set; } = null!;
}