using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Repositories;

public class SportsRegistrationRepository
    : ISportsRegistrationRepository
{
    public Task<SportsRegistration?> GetByIdAsync(
        int sportsRegistrationId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SportsRegistration>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SportsRegistration>>
        GetBySportsEventIdAsync(int sportsEventId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SportsRegistration>>
        GetByStudentIdAsync(int studentId)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(SportsRegistration registration)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(SportsRegistration registration)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(int sportsRegistrationId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByEventAndStudentAsync(
        int sportsEventId,
        int studentId)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountByEventAndStudentDepartmentAsync(
        int sportsEventId,
        int departmentId)
    {
        throw new NotImplementedException();
    }
}