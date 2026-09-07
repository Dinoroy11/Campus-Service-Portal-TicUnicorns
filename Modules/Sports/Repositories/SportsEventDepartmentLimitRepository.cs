using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Repositories;

public class SportsEventDepartmentLimitRepository
    : ISportsEventDepartmentLimitRepository
{
    public Task<SportsEventDepartmentLimit?> GetByIdAsync(
        int sportsEventDepartmentLimitId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SportsEventDepartmentLimit>>
        GetBySportsEventIdAsync(int sportsEventId)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(SportsEventDepartmentLimit departmentLimit)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(SportsEventDepartmentLimit departmentLimit)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(int sportsEventDepartmentLimitId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByEventAndDepartmentAsync(
        int sportsEventId,
        int departmentId)
    {
        throw new NotImplementedException();
    }
}