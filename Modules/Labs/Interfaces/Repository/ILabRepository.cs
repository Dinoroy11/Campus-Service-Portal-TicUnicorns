using CampusServicePortal.Modules.Labs.Entities;

namespace CampusServicePortal.Modules.Labs.Interfaces.Repository;

public interface ILabRepository
{
    Task<List<Lab>> GetAllAsync();

    Task<Lab?> GetByIdAsync(int labId);

    Task<Lab> CreateAsync(Lab lab);

    Task UpdateAsync(Lab lab);

    Task<bool> ExistsAsync(int labId);
}