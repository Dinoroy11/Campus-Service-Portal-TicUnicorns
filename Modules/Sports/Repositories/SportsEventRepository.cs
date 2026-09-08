using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Repositories;

public class SportsEventRepository : ISportsEventRepository
{
    private readonly CampusDbContext _context;

    public SportsEventRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<SportsEvent?> GetByIdAsync(int sportsEventId)
    {
        return await _context.SportsEvents
            .FirstOrDefaultAsync(x => x.SportsEventId == sportsEventId);
    }

    public async Task<IEnumerable<SportsEvent>> GetAllAsync()
    {
        return await _context.SportsEvents
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(SportsEvent sportsEvent)
    {
        await _context.SportsEvents.AddAsync(sportsEvent);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SportsEvent sportsEvent)
    {
        _context.SportsEvents.Update(sportsEvent);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int sportsEventId)
    {
        return await _context.SportsEvents
            .AnyAsync(x => x.SportsEventId == sportsEventId);
    }
}