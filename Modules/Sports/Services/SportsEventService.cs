using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Services;

public class SportsEventService : ISportsEventService
{
    private readonly ISportsEventRepository _sportsEventRepository;

    public SportsEventService(ISportsEventRepository sportsEventRepository)
    {
        _sportsEventRepository = sportsEventRepository;
    }

    public async Task<IEnumerable<SportsEventDto>> GetAllAsync()
    {
        var events = await _sportsEventRepository.GetAllAsync();
        return events
            .OrderByDescending(x => x.EventDate)
            .ThenBy(x => x.StartTime)
            .Select(MapToDto);
    }

    public async Task<IEnumerable<SportsEventDto>> GetAvailableAsync()
    {
        var now = DateTime.UtcNow;
        var events = await _sportsEventRepository.GetAllAsync();

        return events
            .Where(x => x.IsActive)
            .Where(x =>
                x.EventDate.Date > now.Date ||
                (x.EventDate.Date == now.Date && x.EndTime > now.TimeOfDay))
            .OrderBy(x => x.EventDate)
            .ThenBy(x => x.StartTime)
            .Select(MapToDto);
    }

    public async Task<SportsEventDto?> GetByIdAsync(int sportsEventId)
    {
        if (sportsEventId <= 0)
            throw new ArgumentException("Invalid sports event ID.");

        var sportsEvent = await _sportsEventRepository.GetByIdAsync(sportsEventId);
        return sportsEvent == null ? null : MapToDto(sportsEvent);
    }

    public async Task<SportsEventDto> CreateAsync(CreateSportsEventDto dto)
    {
        Validate(dto.EventName, dto.EventDate, dto.StartTime, dto.EndTime);

        var sportsEvent = new SportsEvent
        {
            EventName = dto.EventName.Trim(),
            Description = dto.Description?.Trim(),
            EventDate = dto.EventDate.Date,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Location = dto.Location?.Trim(),
            IsActive = true
        };

        await _sportsEventRepository.AddAsync(sportsEvent);
        return MapToDto(sportsEvent);
    }

    public async Task<SportsEventDto?> UpdateAsync(
        int sportsEventId,
        UpdateSportsEventDto dto)
    {
        if (sportsEventId <= 0)
            throw new ArgumentException("Invalid sports event ID.");

        Validate(dto.EventName, dto.EventDate, dto.StartTime, dto.EndTime);

        var sportsEvent = await _sportsEventRepository.GetByIdAsync(sportsEventId);
        if (sportsEvent == null)
            return null;

        sportsEvent.EventName = dto.EventName.Trim();
        sportsEvent.Description = dto.Description?.Trim();
        sportsEvent.EventDate = dto.EventDate.Date;
        sportsEvent.StartTime = dto.StartTime;
        sportsEvent.EndTime = dto.EndTime;
        sportsEvent.Location = dto.Location?.Trim();
        sportsEvent.IsActive = dto.IsActive;

        await _sportsEventRepository.UpdateAsync(sportsEvent);
        return MapToDto(sportsEvent);
    }

    private static void Validate(
        string eventName,
        DateTime eventDate,
        TimeSpan startTime,
        TimeSpan endTime)
    {
        if (string.IsNullOrWhiteSpace(eventName))
            throw new ArgumentException("Event name is required.");

        if (eventDate == default)
            throw new ArgumentException("Event date is required.");

        if (startTime >= endTime)
            throw new ArgumentException("Start time must be earlier than end time.");
    }

    private static SportsEventDto MapToDto(SportsEvent sportsEvent)
    {
        return new SportsEventDto
        {
            SportsEventId = sportsEvent.SportsEventId,
            EventName = sportsEvent.EventName,
            Description = sportsEvent.Description,
            EventDate = sportsEvent.EventDate,
            StartTime = sportsEvent.StartTime,
            EndTime = sportsEvent.EndTime,
            Location = sportsEvent.Location,
            IsActive = sportsEvent.IsActive
        };
    }
}
