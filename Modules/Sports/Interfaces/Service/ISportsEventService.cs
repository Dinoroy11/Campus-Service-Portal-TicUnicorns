using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;

public interface ISportsEventService
{
    Task<IEnumerable<SportsEventDto>> GetAllAsync();

    Task<SportsEventDto?> GetByIdAsync(int sportsEventId);

    Task<SportsEventDto> CreateAsync(CreateSportsEventDto dto);

    Task<SportsEventDto?> UpdateAsync(
        int sportsEventId,
        UpdateSportsEventDto dto);
}
