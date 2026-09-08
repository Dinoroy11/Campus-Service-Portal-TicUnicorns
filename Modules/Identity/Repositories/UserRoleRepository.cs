using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Identity.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly CampusDbContext _context;

    public UserRoleRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(int userId, int roleId)
    {
        return await _context.UserRoles
            .AnyAsync(x => x.UserId == userId && x.RoleId == roleId);
    }

    public async Task AddAsync(UserRole userRole)
    {
        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();
    }
}