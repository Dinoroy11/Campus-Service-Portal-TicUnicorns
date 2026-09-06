using CampusServicePortal.Modules.Events.Entities;

namespace CampusServicePortal.Modules.Events.Repositories;

public interface IEventSeatRepository
{
    Task<List<EventSeat>> GetAllAsync();

    Task<List<EventSeat>> GetByEventIdAsync(int eventId);

    Task<EventSeat?> GetByIdAsync(int eventSeatId);

    Task<EventSeat> CreateAsync(EventSeat eventSeat);

    Task UpdateAsync(EventSeat eventSeat);

    Task<bool> ExistsAsync(int eventSeatId);
}