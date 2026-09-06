using CampusServicePortal.Modules.Events.Entities;

namespace CampusServicePortal.Modules.Events.Repositories;

public interface IVenueRepository
{
    Task<List<Venue>> GetAllAsync();

    Task<Venue?> GetByIdAsync(int venueId);

    Task<Venue> CreateAsync(Venue venue);

    Task UpdateAsync(Venue venue);

    Task<bool> ExistsAsync(int venueId);

    Task<int?> GetCapacityAsync(int venueId);
}