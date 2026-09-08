using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Repositories;

public class CoachMeetingRepository : ICoachMeetingRepository
{
    private readonly CampusDbContext _context;

    public CoachMeetingRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<CoachMeeting?> GetByIdAsync(int coachMeetingId)
    {
        return await _context.CoachMeetings
            .FirstOrDefaultAsync(x =>
                x.CoachMeetingId == coachMeetingId);
    }

    public async Task<IEnumerable<CoachMeeting>> GetAllAsync()
    {
        return await _context.CoachMeetings
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<CoachMeeting>>
        GetBySportsEventIdAsync(int sportsEventId)
    {
        return await _context.CoachMeetings
            .AsNoTracking()
            .Where(x => x.SportsEventId == sportsEventId)
            .ToListAsync();
    }

    public async Task AddAsync(CoachMeeting coachMeeting)
    {
        await _context.CoachMeetings
            .AddAsync(coachMeeting);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CoachMeeting coachMeeting)
    {
        _context.CoachMeetings
            .Update(coachMeeting);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int coachMeetingId)
    {
        return await _context.CoachMeetings
            .AnyAsync(x =>
                x.CoachMeetingId == coachMeetingId);
    }
}