using CampusServicePortal.Modules.Events.Entities;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Events.Repositories;

public class EventRegistrationRepository : IEventRegistrationRepository
{
    private readonly CampusDbContext _context;

    public EventRegistrationRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<EventRegistration>> GetAllAsync()
    {
        return await _context.Set<EventRegistration>()
            .OrderByDescending(x => x.RegisteredAt)
            .ToListAsync();
    }

    public async Task<EventRegistration?> GetByIdAsync(int eventRegistrationId)
    {
        return await _context.Set<EventRegistration>()
            .FirstOrDefaultAsync(er => er.EventRegistrationId == eventRegistrationId);
    }

    public async Task<List<EventRegistration>> GetByEventIdAsync(int eventId)
    {
        return await _context.Set<EventRegistration>()
            .Where(er => er.EventId == eventId)
            .OrderByDescending(x => x.RegisteredAt)
            .ToListAsync();
    }

    public async Task<List<EventRegistration>> GetByStudentIdAsync(int studentId)
    {
        return await _context.Set<EventRegistration>()
            .Where(er => er.StudentId == studentId)
            .OrderByDescending(x => x.RegisteredAt)
            .ToListAsync();
    }

    public async Task<EventRegistration?> GetByEventAndStudentAsync(
        int eventId,
        int studentId)
    {
        return await _context.Set<EventRegistration>()
            .FirstOrDefaultAsync(x =>
                x.EventId == eventId &&
                x.StudentId == studentId);
    }

    public async Task<EventRegistration> CreateAsync(EventRegistration registration)
    {
        _context.Set<EventRegistration>().Add(registration);
        await _context.SaveChangesAsync();
        return registration;
    }

    public async Task UpdateAsync(EventRegistration registration)
    {
        _context.Set<EventRegistration>().Update(registration);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int eventRegistrationId)
    {
        return await _context.Set<EventRegistration>()
            .AnyAsync(er => er.EventRegistrationId == eventRegistrationId);
    }
}
