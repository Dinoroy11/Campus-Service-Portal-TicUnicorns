using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Identity.Interfaces.Repository;


namespace CampusServicePortal_TicUnicorns.Modules.Identity.Repositories;

public class PermissionRepository : IPermissionRepository
{
    public Task<Permission?> GetByIdAsync(int permissionId)
    {
        throw new NotImplementedException();
    }

    public Task<Permission?> GetByCodeAsync(string code)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Permission>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Permission permission)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Permission permission)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(int permissionId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByCodeAsync(string code)
    {
        throw new NotImplementedException();
    }
}