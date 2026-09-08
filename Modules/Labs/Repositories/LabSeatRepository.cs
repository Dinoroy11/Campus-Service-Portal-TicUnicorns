using CampusServicePortal.Modules.Labs.Entities;
using CampusServicePortal.Modules.Labs.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Labs.Repositories;

public class LabSeatRepository : ILabSeatRepository
{
    private readonly CampusDbContext _context;

    public LabSeatRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<LabSeat>> GetByLabIdAsync(int labId)
    {
        return await _context.Set<LabSeat>()
            .AsNoTracking()
            .Where(x => x.LabId == labId)
            .ToListAsync();
    }

    public async Task<LabSeat?> GetByIdAsync(int labSeatId)
    {
        return await _context.Set<LabSeat>()
            .FirstOrDefaultAsync(x => x.LabSeatId == labSeatId);
    }

    public async Task<LabSeat> CreateAsync(LabSeat labSeat)
    {
        await _context.Set<LabSeat>().AddAsync(labSeat);
        await _context.SaveChangesAsync();

        return labSeat;
    }

    public async Task UpdateAsync(LabSeat labSeat)
    {
        _context.Set<LabSeat>().Update(labSeat);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int labSeatId)
    {
        return await _context.Set<LabSeat>()
            .AnyAsync(x => x.LabSeatId == labSeatId);
    }
}