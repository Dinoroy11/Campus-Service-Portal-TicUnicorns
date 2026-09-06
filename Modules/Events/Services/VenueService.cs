using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Entities;
using CampusServicePortal.Modules.Events.Repositories;

namespace CampusServicePortal.Modules.Events.Services;

public class VenueService : IVenueService
{
    private readonly IVenueRepository _venueRepository;

    public VenueService(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<List<VenueDto>> GetAllAsync()
    {
        var venues = await _venueRepository.GetAllAsync();

        return venues.Select(v => new VenueDto
        {
            VenueId = v.VenueId,
            VenueName = v.VenueName,
            Capacity = v.Capacity,
            Description = v.Description,
            IsActive = v.IsActive
        }).ToList();
    }

    public async Task<VenueDto?> GetByIdAsync(int venueId)
    {
        var venue = await _venueRepository.GetByIdAsync(venueId);

        if (venue == null)
            return null;

        return new VenueDto
        {
            VenueId = venue.VenueId,
            VenueName = venue.VenueName,
            Capacity = venue.Capacity,
            Description = venue.Description,
            IsActive = venue.IsActive
        };
    }

    public async Task<VenueDto> CreateAsync(CreateVenueDto dto)
    {
        if (dto.Capacity <= 0)
            throw new ArgumentException("Venue capacity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(dto.VenueName))
            throw new ArgumentException("Venue name is required.");

        var venue = new Venue
        {
            VenueName = dto.VenueName.Trim(),
            Capacity = dto.Capacity,
            Description = dto.Description,
            IsActive = dto.IsActive
        };

        var createdVenue = await _venueRepository.CreateAsync(venue);

        return new VenueDto
        {
            VenueId = createdVenue.VenueId,
            VenueName = createdVenue.VenueName,
            Capacity = createdVenue.Capacity,
            Description = createdVenue.Description,
            IsActive = createdVenue.IsActive
        };
    }

    public async Task<bool> UpdateAsync(int venueId, UpdateVenueDto dto)
    {
        var venue = await _venueRepository.GetByIdAsync(venueId);

        if (venue == null)
            return false;

        if (dto.Capacity <= 0)
            throw new ArgumentException("Venue capacity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(dto.VenueName))
            throw new ArgumentException("Venue name is required.");

        venue.VenueName = dto.VenueName.Trim();
        venue.Capacity = dto.Capacity;
        venue.Description = dto.Description;
        venue.IsActive = dto.IsActive;

        await _venueRepository.UpdateAsync(venue);

        return true;
    }
}