using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Identity.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Repositories;

public class DepartmentStaffAssignmentRepository
    : IDepartmentStaffAssignmentRepository
{
    private readonly CampusDbContext _context;

    public DepartmentStaffAssignmentRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(int userId, int departmentId)
    {
        return await _context.DepartmentStaffAssignments
            .AnyAsync(x =>
                x.UserId == userId &&
                x.DepartmentId == departmentId);
    }

    public async Task AddAsync(DepartmentStaffAssignment assignment)
    {
        await _context.DepartmentStaffAssignments.AddAsync(assignment);
        await _context.SaveChangesAsync();
    }
}