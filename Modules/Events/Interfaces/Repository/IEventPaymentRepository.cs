using CampusServicePortal.Modules.Events.Entities;

namespace CampusServicePortal.Modules.Events.Repositories;

public interface IEventPaymentRepository
{
    Task<List<EventPayment>> GetAllAsync();

    Task<EventPayment?> GetByIdAsync(int eventPaymentId);

    Task<EventPayment?> GetByRegistrationIdAsync(int registrationId);

    Task<EventPayment> CreateAsync(EventPayment payment);

    Task UpdateAsync(EventPayment payment);

    Task<bool> ExistsAsync(int eventPaymentId);
}