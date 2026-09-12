using CampusServicePortal.Modules.Events.Entities;

namespace CampusServicePortal.Modules.Events.Repositories;

public interface IEventRepository
{
    Task<List<Event>> GetAllAsync();
    Task<List<Event>> GetByVenueIdAsync(int venueId);
    Task<Event?> GetByIdAsync(int eventId);
    Task<Event> CreateAsync(Event eventEntity);
    Task UpdateAsync(Event eventEntity);
    Task<bool> ExistsAsync(int eventId);
}
