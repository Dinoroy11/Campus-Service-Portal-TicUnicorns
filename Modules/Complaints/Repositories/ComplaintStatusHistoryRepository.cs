using CampusServicePortal.Modules.Complaints.Entities;
using CampusServicePortal.Modules.Complaints.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Complaints.Repositories;

public class ComplaintStatusHistoryRepository
    : IComplaintStatusHistoryRepository
{
    private readonly CampusDbContext _context;

    public ComplaintStatusHistoryRepository(
        CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<ComplaintStatusHistory>>
        GetByComplaintIdAsync(int complaintId)
    {
        return await _context.Set<ComplaintStatusHistory>()
            .AsNoTracking()
            .Where(x => x.ComplaintId == complaintId)
            .OrderByDescending(x => x.ChangedAt)
            .ToListAsync();
    }

    public async Task<ComplaintStatusHistory> CreateAsync(
        ComplaintStatusHistory history)
    {
        await _context.Set<ComplaintStatusHistory>()
            .AddAsync(history);

        await _context.SaveChangesAsync();

        return history;
    }
}