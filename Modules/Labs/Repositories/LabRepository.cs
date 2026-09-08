using CampusServicePortal.Modules.Labs.Entities;
using CampusServicePortal.Modules.Labs.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Labs.Repositories;

public class LabRepository : ILabRepository
{
    private readonly CampusDbContext _context;

    public LabRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<Lab>> GetAllAsync()
    {
        return await _context.Set<Lab>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Lab?> GetByIdAsync(int labId)
    {
        return await _context.Set<Lab>()
            .FirstOrDefaultAsync(x => x.LabId == labId);
    }

    public async Task<Lab> CreateAsync(Lab lab)
    {
        await _context.Set<Lab>().AddAsync(lab);
        await _context.SaveChangesAsync();

        return lab;
    }

    public async Task UpdateAsync(Lab lab)
    {
        _context.Set<Lab>().Update(lab);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int labId)
    {
        return await _context.Set<Lab>()
            .AnyAsync(x => x.LabId == labId);
    }
}