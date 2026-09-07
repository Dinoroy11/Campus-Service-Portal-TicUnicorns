using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;

public interface ISportsEventRepository
{
    Task<SportsEvent?> GetByIdAsync(int sportsEventId);

    Task<IEnumerable<SportsEvent>> GetAllAsync();

    Task AddAsync(SportsEvent sportsEvent);

    Task UpdateAsync(SportsEvent sportsEvent);

    Task<bool> ExistsAsync(int sportsEventId);
}