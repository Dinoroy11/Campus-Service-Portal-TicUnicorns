using CampusServicePortal.Modules.Complaints.Entities;
using CampusServicePortal.Modules.Complaints.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Complaints.Repositories;

public class ComplaintCategoryRepository : IComplaintCategoryRepository
{
    private readonly CampusDbContext _context;

    public ComplaintCategoryRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<ComplaintCategory>> GetAllAsync()
    {
        return await _context.Set<ComplaintCategory>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ComplaintCategory?> GetByIdAsync(
        int complaintCategoryId)
    {
        return await _context.Set<ComplaintCategory>()
            .FirstOrDefaultAsync(x =>
                x.ComplaintCategoryId == complaintCategoryId);
    }

    public async Task<ComplaintCategory> CreateAsync(
        ComplaintCategory category)
    {
        await _context.Set<ComplaintCategory>().AddAsync(category);
        await _context.SaveChangesAsync();

        return category;
    }

    public async Task UpdateAsync(ComplaintCategory category)
    {
        _context.Set<ComplaintCategory>().Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int complaintCategoryId)
    {
        return await _context.Set<ComplaintCategory>()
            .AnyAsync(x =>
                x.ComplaintCategoryId == complaintCategoryId);
    }
}