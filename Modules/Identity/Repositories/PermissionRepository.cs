using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Identity.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly CampusDbContext _context;

    public PermissionRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<Permission?> GetByIdAsync(int permissionId)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(p => p.PermissionId == permissionId);
    }

    public async Task<Permission?> GetByCodeAsync(string code)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(p => p.Code == code);
    }

    public async Task<IEnumerable<Permission>> GetAllAsync()
    {
        return await _context.Permissions
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Permission permission)
    {
        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Permission permission)
    {
        _context.Permissions.Update(permission);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int permissionId)
    {
        return await _context.Permissions
            .AnyAsync(p => p.PermissionId == permissionId);
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _context.Permissions
            .AnyAsync(p => p.Code == code);
    }
}