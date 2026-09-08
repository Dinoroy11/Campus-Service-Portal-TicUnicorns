using CampusServicePortal.Modules.Identity.Entities;

namespace CampusServicePortal.Modules.Identity.Interfaces.Repository;

public interface IRolePermissionRepository
{
    Task<bool> ExistsAsync(int roleId, int permissionId);

    Task AddAsync(RolePermission rolePermission);
}