namespace CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;

public class SportsEventDepartmentLimitDto
{
    public int SportsEventDepartmentLimitId { get; set; }

    public int SportsEventId { get; set; }

    public int DepartmentId { get; set; }

    public int RegistrationLimit { get; set; }
}