using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal.Modules.Hostels.Entities;
 
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Hostels.Repositories;

public class HostelRepository : IHostelRepository
{
    private readonly CampusDbContext _context;

    public HostelRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<Hostel>> GetHostelsAsync()
    {
        return await _context.Hostels
            .Where(h => h.IsActive)
            .ToListAsync();
    }

    public async Task<Hostel?> GetHostelBlueprintAsync(int hostelId)
    {
        return await _context.Hostels
            .Where(h => h.HostelId == hostelId && h.IsActive)
            .Include(h => h.Floors)
                .ThenInclude(f => f.Rooms)
                    .ThenInclude(r => r.RoomBeds)
            .FirstOrDefaultAsync();
    }
}