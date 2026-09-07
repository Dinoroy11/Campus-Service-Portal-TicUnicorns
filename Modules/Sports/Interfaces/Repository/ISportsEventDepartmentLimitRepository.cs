using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;

public interface ISportsEventDepartmentLimitRepository
{
    Task<SportsEventDepartmentLimit?> GetByIdAsync(
        int sportsEventDepartmentLimitId);

    Task<IEnumerable<SportsEventDepartmentLimit>> GetBySportsEventIdAsync(
        int sportsEventId);

    Task AddAsync(SportsEventDepartmentLimit departmentLimit);

    Task UpdateAsync(SportsEventDepartmentLimit departmentLimit);

    Task<bool> ExistsAsync(int sportsEventDepartmentLimitId);

    Task<bool> ExistsByEventAndDepartmentAsync(
        int sportsEventId,
        int departmentId);
}