using CampusServicePortal.Modules.Events.Entities;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Events.Repositories;

public class EventRepository : IEventRepository
{
    private readonly CampusDbContext _context;

    public EventRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<Event>> GetAllAsync()
    {
        return await _context.Set<Event>()
            .AsNoTracking()
            .OrderBy(x => x.StartDateTime)
            .ToListAsync();
    }

    public async Task<List<Event>> GetByVenueIdAsync(int venueId)
    {
        return await _context.Set<Event>()
            .AsNoTracking()
            .Where(x => x.VenueId == venueId)
            .OrderBy(x => x.StartDateTime)
            .ToListAsync();
    }

    public async Task<Event?> GetByIdAsync(int eventId)
    {
        return await _context.Set<Event>()
            .FirstOrDefaultAsync(e => e.EventId == eventId);
    }

    public async Task<Event> CreateAsync(Event eventEntity)
    {
        _context.Set<Event>().Add(eventEntity);
        await _context.SaveChangesAsync();
        return eventEntity;
    }

    public async Task UpdateAsync(Event eventEntity)
    {
        _context.Set<Event>().Update(eventEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int eventId)
    {
        return await _context.Set<Event>()
            .AnyAsync(e => e.EventId == eventId);
    }
}
