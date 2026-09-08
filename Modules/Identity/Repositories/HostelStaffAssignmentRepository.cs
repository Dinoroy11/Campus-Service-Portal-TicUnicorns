using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Identity.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Repositories;

public class HostelStaffAssignmentRepository
    : IHostelStaffAssignmentRepository
{
    private readonly CampusDbContext _context;

    public HostelStaffAssignmentRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(int userId, int hostelId)
    {
        return await _context.HostelStaffAssignments
            .AnyAsync(x =>
                x.UserId == userId &&
                x.HostelId == hostelId);
    }

    public async Task AddAsync(HostelStaffAssignment assignment)
    {
        await _context.HostelStaffAssignments.AddAsync(assignment);
        await _context.SaveChangesAsync();
    }
}