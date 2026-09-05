using CampusServicePortal.Modules.Identity.Entities;

namespace CampusServicePortal.Modules.Identity.Interfaces.Repository;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int roleId);

    Task<IEnumerable<Role>> GetAllAsync();

    Task AddAsync(Role role);

    Task UpdateAsync(Role role);

    Task<bool> ExistsAsync(int roleId);

    Task<bool> ExistsByNameAsync(string roleName);
}