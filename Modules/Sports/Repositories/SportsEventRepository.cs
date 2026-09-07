using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Repositories;

public class SportsEventRepository : ISportsEventRepository
{
    public Task<SportsEvent?> GetByIdAsync(int sportsEventId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SportsEvent>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(SportsEvent sportsEvent)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(SportsEvent sportsEvent)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(int sportsEventId)
    {
        throw new NotImplementedException();
    }
}