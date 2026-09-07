using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;

public interface ISportsRegistrationRepository
{
    Task<SportsRegistration?> GetByIdAsync(
        int sportsRegistrationId);

    Task<IEnumerable<SportsRegistration>> GetAllAsync();

    Task<IEnumerable<SportsRegistration>> GetBySportsEventIdAsync(
        int sportsEventId);

    Task<IEnumerable<SportsRegistration>> GetByStudentIdAsync(
        int studentId);

    Task AddAsync(SportsRegistration registration);

    Task UpdateAsync(SportsRegistration registration);

    Task<bool> ExistsAsync(int sportsRegistrationId);

    Task<bool> ExistsByEventAndStudentAsync(
        int sportsEventId,
        int studentId);

    Task<int> CountByEventAndStudentDepartmentAsync(
        int sportsEventId,
        int departmentId);
}