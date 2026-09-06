using CampusServicePortal.Modules.Events.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Events.Interfaces.Service;

public interface IEventRegistrationService
{
    Task<List<EventRegistrationDto>> GetAllAsync();

    Task<EventRegistrationDto?> GetByIdAsync(int eventRegistrationId);

    Task<List<EventRegistrationDto>> GetByEventIdAsync(int eventId);

    Task<List<EventRegistrationDto>> GetByStudentIdAsync(int studentId);

    Task<EventRegistrationDto> RegisterAsync(EventRegistrationDto dto);

    Task<bool> UpdateAsync(
        int eventRegistrationId,
        EventRegistrationDto dto);
}