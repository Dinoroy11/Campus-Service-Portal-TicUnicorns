using CampusServicePortal.Modules.Labs.Entities;
using CampusServicePortal.Modules.Labs.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Labs.Repositories;

public class LabTimeSlotRepository : ILabTimeSlotRepository
{
    private readonly CampusDbContext _context;

    public LabTimeSlotRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<LabTimeSlot>> GetByLabIdAsync(int labId)
    {
        return await _context.Set<LabTimeSlot>()
            .AsNoTracking()
            .Where(x => x.LabId == labId)
            .OrderBy(x => x.StartTime)
            .ToListAsync();
    }

    public async Task<LabTimeSlot?> GetByIdAsync(int timeSlotId)
    {
        return await _context.Set<LabTimeSlot>()
            .FirstOrDefaultAsync(x => x.TimeSlotId == timeSlotId);
    }

    public async Task<LabTimeSlot> CreateAsync(LabTimeSlot timeSlot)
    {
        await _context.Set<LabTimeSlot>().AddAsync(timeSlot);
        await _context.SaveChangesAsync();

        return timeSlot;
    }

    public async Task UpdateAsync(LabTimeSlot timeSlot)
    {
        _context.Set<LabTimeSlot>().Update(timeSlot);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int timeSlotId)
    {
        return await _context.Set<LabTimeSlot>()
            .AnyAsync(x => x.TimeSlotId == timeSlotId);
    }
}