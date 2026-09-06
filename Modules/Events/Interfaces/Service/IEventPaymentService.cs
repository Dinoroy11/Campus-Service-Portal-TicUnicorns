using CampusServicePortal.Modules.Events.DTOs;
 

namespace CampusServicePortal.Modules.Events.Services;

public interface IEventPaymentService
{
    Task<List<EventPaymentDto>> GetAllAsync();

    Task<EventPaymentDto?> GetByIdAsync(int eventPaymentId);

    Task<EventPaymentDto?> GetByRegistrationIdAsync(int registrationId);

    Task<EventPaymentDto> CreateAsync(EventPaymentDto dto);

    Task<bool> UpdateAsync(int eventPaymentId, EventPaymentDto dto);
}