using CampusServicePortal.Modules.Complaints.Entities;
using CampusServicePortal.Modules.Complaints.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Complaints.Repositories;

public class ComplaintRepository : IComplaintRepository
{
    private readonly CampusDbContext _context;

    public ComplaintRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<Complaint>> GetAllAsync()
    {
        return await _context.Set<Complaint>()
            .AsNoTracking()
            .Include(x => x.Category)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Complaint?> GetByIdAsync(int complaintId)
    {
        return await _context.Set<Complaint>()
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x =>
                x.ComplaintId == complaintId);
    }

    public async Task<List<Complaint>> GetByStudentIdAsync(
        int studentId)
    {
        return await _context.Set<Complaint>()
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Complaint> CreateAsync(Complaint complaint)
    {
        await _context.Set<Complaint>().AddAsync(complaint);
        await _context.SaveChangesAsync();

        return complaint;
    }

    public async Task UpdateAsync(Complaint complaint)
    {
        _context.Set<Complaint>().Update(complaint);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int complaintId)
    {
        return await _context.Set<Complaint>()
            .AnyAsync(x => x.ComplaintId == complaintId);
    }
}
