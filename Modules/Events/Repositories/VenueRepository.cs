using CampusServicePortal.Modules.Events.Entities;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Events.Repositories;

public class VenueRepository : IVenueRepository
{
    private readonly CampusDbContext _context;

    public VenueRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<Venue>> GetAllAsync()
    {
        return await _context.Set<Venue>()
            .ToListAsync();
    }

    public async Task<Venue?> GetByIdAsync(int venueId)
    {
        return await _context.Set<Venue>()
            .FirstOrDefaultAsync(v => v.VenueId == venueId);
    }

    public async Task<Venue> CreateAsync(Venue venue)
    {
        _context.Set<Venue>().Add(venue);
        await _context.SaveChangesAsync();

        return venue;
    }

    public async Task UpdateAsync(Venue venue)
    {
        _context.Set<Venue>().Update(venue);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int venueId)
    {
        return await _context.Set<Venue>()
            .AnyAsync(v => v.VenueId == venueId);
    }

    public async Task<int?> GetCapacityAsync(int venueId)
    {
        var venue = await _context.Set<Venue>()
            .FirstOrDefaultAsync(v => v.VenueId == venueId);

        return venue?.Capacity;
    }
}