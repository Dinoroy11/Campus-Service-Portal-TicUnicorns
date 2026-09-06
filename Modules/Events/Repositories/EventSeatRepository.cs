using CampusServicePortal.Modules.Events.Entities;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Events.Repositories;

public class EventSeatRepository : IEventSeatRepository
{
    private readonly CampusDbContext _context;

    public EventSeatRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<EventSeat>> GetAllAsync()
    {
        return await _context.Set<EventSeat>()
            .ToListAsync();
    }

    public async Task<List<EventSeat>> GetByEventIdAsync(int eventId)
    {
        return await _context.Set<EventSeat>()
            .Where(es => es.EventId == eventId)
            .ToListAsync();
    }

    public async Task<EventSeat?> GetByIdAsync(int eventSeatId)
    {
        return await _context.Set<EventSeat>()
            .FirstOrDefaultAsync(es => es.EventSeatId == eventSeatId);
    }

    public async Task<EventSeat> CreateAsync(EventSeat eventSeat)
    {
        _context.Set<EventSeat>().Add(eventSeat);
        await _context.SaveChangesAsync();

        return eventSeat;
    }

    public async Task UpdateAsync(EventSeat eventSeat)
    {
        _context.Set<EventSeat>().Update(eventSeat);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int eventSeatId)
    {
        return await _context.Set<EventSeat>()
            .AnyAsync(es => es.EventSeatId == eventSeatId);
    }
}