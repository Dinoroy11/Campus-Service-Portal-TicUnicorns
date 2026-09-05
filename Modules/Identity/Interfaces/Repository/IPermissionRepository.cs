using CampusServicePortal.Modules.Identity.Entities;

namespace CampusServicePortal.Modules.Identity.Interfaces.Repository;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(int permissionId);

    Task<Permission?> GetByCodeAsync(string code);

    Task<IEnumerable<Permission>> GetAllAsync();

    Task AddAsync(Permission permission);

    Task UpdateAsync(Permission permission);

    Task<bool> ExistsAsync(int permissionId);

    Task<bool> ExistsByCodeAsync(string code);
}