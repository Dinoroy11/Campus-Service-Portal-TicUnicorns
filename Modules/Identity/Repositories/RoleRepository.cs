using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Identity.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly CampusDbContext _context;

    public RoleRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(int roleId)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleId == roleId);
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Role role)
    {
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int roleId)
    {
        return await _context.Roles
            .AnyAsync(r => r.RoleId == roleId);
    }

    public async Task<bool> ExistsByNameAsync(string roleName)
    {
        return await _context.Roles
            .AnyAsync(r => r.RoleName == roleName);
    }
}