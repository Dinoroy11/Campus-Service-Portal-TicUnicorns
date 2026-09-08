using CampusServicePortal.Modules.Identity.Entities;

namespace CampusServicePortal.Modules.Identity.Interfaces.Repository;

public interface IUserRoleRepository
{
    Task<bool> ExistsAsync(int userId, int roleId);

    Task AddAsync(UserRole userRole);
}