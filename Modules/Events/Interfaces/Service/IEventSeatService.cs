using CampusServicePortal.Modules.Events.DTOs;

namespace CampusServicePortal.Modules.Events.Services;

public interface IEventSeatService
{
    Task<List<EventSeatDto>> GetAllAsync();

    Task<List<EventSeatDto>> GetByEventIdAsync(int eventId);

    Task<EventSeatDto?> GetByIdAsync(int eventSeatId);

    Task<EventSeatDto> CreateAsync(EventSeatDto dto);

    Task<bool> UpdateAsync(int eventSeatId, EventSeatDto dto);
}