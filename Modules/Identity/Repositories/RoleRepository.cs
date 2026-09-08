using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Identity.Interfaces.Repository;


namespace CampusServicePortal_TicUnicorns.Modules.Identity.Repositories;

public class RoleRepository : IRoleRepository
{
    public Task<Role?> GetByIdAsync(int roleId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Role>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Role role)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Role role)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(int roleId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByNameAsync(string roleName)
    {
        throw new NotImplementedException();
    }
}