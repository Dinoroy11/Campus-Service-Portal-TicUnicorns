using CampusServicePortal.Modules.Events.DTOs;

namespace CampusServicePortal.Modules.Events.Services;

public interface IEventService
{
    Task<List<EventDto>> GetAllAsync();

    Task<EventDto?> GetByIdAsync(int eventId);

    Task<EventDto> CreateAsync(CreateEventDto dto);

    Task<bool> UpdateAsync(int eventId, UpdateEventDto dto);
}