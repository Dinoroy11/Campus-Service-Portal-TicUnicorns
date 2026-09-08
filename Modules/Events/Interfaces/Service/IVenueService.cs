using CampusServicePortal.Modules.Events.DTOs;

namespace CampusServicePortal.Modules.Events.Services;

public interface IVenueService
{
    Task<List<VenueDto>> GetAllAsync();

    Task<VenueDto?> GetByIdAsync(int venueId);

    Task<VenueDto> CreateAsync(CreateVenueDto dto);

    Task<bool> UpdateAsync(int venueId, UpdateVenueDto dto);
}