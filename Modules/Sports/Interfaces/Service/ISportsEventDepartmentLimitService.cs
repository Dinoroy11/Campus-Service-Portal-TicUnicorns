using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;

public interface ISportsEventDepartmentLimitService
{
    Task<IEnumerable<SportsEventDepartmentLimitDto>>
        GetBySportsEventIdAsync(int sportsEventId);

    Task<SportsEventDepartmentLimitDto>
        CreateAsync(SportsEventDepartmentLimitDto dto);

    Task<SportsEventDepartmentLimitDto?>
        UpdateAsync(
            int sportsEventDepartmentLimitId,
            SportsEventDepartmentLimitDto dto);
}