using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Identity.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Repositories;

public class CanteenStaffAssignmentRepository
    : ICanteenStaffAssignmentRepository
{
    private readonly CampusDbContext _context;

    public CanteenStaffAssignmentRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(int userId, int canteenId)
    {
        return await _context.CanteenStaffAssignments
            .AnyAsync(x =>
                x.UserId == userId &&
                x.CanteenId == canteenId);
    }

    public async Task AddAsync(CanteenStaffAssignment assignment)
    {
        await _context.CanteenStaffAssignments.AddAsync(assignment);
        await _context.SaveChangesAsync();
    }
}