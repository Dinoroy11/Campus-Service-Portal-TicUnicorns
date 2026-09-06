using CampusServicePortal.Modules.Events.Entities;

namespace CampusServicePortal.Modules.Events.Repositories;

public interface IEventRegistrationRepository
{
    Task<List<EventRegistration>> GetAllAsync();

    Task<EventRegistration?> GetByIdAsync(int eventRegistrationId);

    Task<List<EventRegistration>> GetByEventIdAsync(int eventId);

    Task<List<EventRegistration>> GetByStudentIdAsync(int studentId);

    Task<EventRegistration> CreateAsync(EventRegistration registration);

    Task UpdateAsync(EventRegistration registration);

    Task<bool> ExistsAsync(int eventRegistrationId);
}