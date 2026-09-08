using CampusServicePortal.Modules.Events.Entities;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Events.Repositories;

public class EventPaymentRepository : IEventPaymentRepository
{
    private readonly CampusDbContext _context;

    public EventPaymentRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<EventPayment>> GetAllAsync()
    {
        return await _context.Set<EventPayment>()
            .ToListAsync();
    }

    public async Task<EventPayment?> GetByIdAsync(int eventPaymentId)
    {
        return await _context.Set<EventPayment>()
            .FirstOrDefaultAsync(ep => ep.EventPaymentId == eventPaymentId);
    }

    public async Task<EventPayment?> GetByRegistrationIdAsync(int registrationId)
    {
        return await _context.Set<EventPayment>()
            .FirstOrDefaultAsync(ep => ep.RegistrationId == registrationId);
    }

    public async Task<EventPayment> CreateAsync(EventPayment payment)
    {
        _context.Set<EventPayment>().Add(payment);
        await _context.SaveChangesAsync();

        return payment;
    }

    public async Task UpdateAsync(EventPayment payment)
    {
        _context.Set<EventPayment>().Update(payment);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int eventPaymentId)
    {
        return await _context.Set<EventPayment>()
            .AnyAsync(ep => ep.EventPaymentId == eventPaymentId);
    }
}