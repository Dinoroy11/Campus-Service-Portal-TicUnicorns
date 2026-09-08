using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Identity.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Repositories;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly CampusDbContext _context;

    public RolePermissionRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(int roleId, int permissionId)
    {
        return await _context.RolePermissions
            .AnyAsync(x =>
                x.RoleId == roleId &&
                x.PermissionId == permissionId);
    }

    public async Task AddAsync(RolePermission rolePermission)
    {
        await _context.RolePermissions.AddAsync(rolePermission);
        await _context.SaveChangesAsync();
    }
}