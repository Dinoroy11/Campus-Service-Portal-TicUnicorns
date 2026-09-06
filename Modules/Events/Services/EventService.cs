using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Entities;
using CampusServicePortal.Modules.Events.Repositories;

namespace CampusServicePortal.Modules.Events.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IVenueRepository _venueRepository;

    public EventService(
        IEventRepository eventRepository,
        IVenueRepository venueRepository)
    {
        _eventRepository = eventRepository;
        _venueRepository = venueRepository;
    }

    public async Task<List<EventDto>> GetAllAsync()
    {
        var events = await _eventRepository.GetAllAsync();

        return events.Select(MapToDto).ToList();
    }

    public async Task<EventDto?> GetByIdAsync(int eventId)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);

        if (eventEntity == null)
            return null;

        return MapToDto(eventEntity);
    }

    public async Task<EventDto> CreateAsync(CreateEventDto dto)
    {
        ValidateEventDates(dto.StartDateTime, dto.EndDateTime);

        ValidatePaymentDetails(
            dto.IsPaid,
            dto.FeeAmount);

        ValidateHoldDuration(dto.HoldDurationMinutes);

        var venue = await _venueRepository.GetByIdAsync(dto.VenueId);

        if (venue == null)
            throw new ArgumentException("Venue not found.");

        if (!venue.IsActive)
            throw new ArgumentException("Selected venue is not active.");

        var eventEntity = new Event
        {
            VenueId = dto.VenueId,
            EventName = dto.EventName.Trim(),
            Description = dto.Description,
            StartDateTime = dto.StartDateTime,
            EndDateTime = dto.EndDateTime,
            IsPaid = dto.IsPaid,
            UsesReservedSeating = dto.UsesReservedSeating,
            FeeAmount = dto.FeeAmount,
            HoldDurationMinutes = dto.HoldDurationMinutes,
            IsActive = dto.IsActive
        };

        var createdEvent = await _eventRepository.CreateAsync(eventEntity);

        return MapToDto(createdEvent);
    }

    public async Task<bool> UpdateAsync(int eventId, UpdateEventDto dto)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);

        if (eventEntity == null)
            return false;

        ValidateEventDates(dto.StartDateTime, dto.EndDateTime);

        ValidatePaymentDetails(
            dto.IsPaid,
            dto.FeeAmount);

        ValidateHoldDuration(dto.HoldDurationMinutes);

        var venue = await _venueRepository.GetByIdAsync(dto.VenueId);

        if (venue == null)
            throw new ArgumentException("Venue not found.");

        if (!venue.IsActive)
            throw new ArgumentException("Selected venue is not active.");

        eventEntity.VenueId = dto.VenueId;
        eventEntity.EventName = dto.EventName.Trim();
        eventEntity.Description = dto.Description;
        eventEntity.StartDateTime = dto.StartDateTime;
        eventEntity.EndDateTime = dto.EndDateTime;
        eventEntity.IsPaid = dto.IsPaid;
        eventEntity.UsesReservedSeating = dto.UsesReservedSeating;
        eventEntity.FeeAmount = dto.FeeAmount;
        eventEntity.HoldDurationMinutes = dto.HoldDurationMinutes;
        eventEntity.IsActive = dto.IsActive;

        await _eventRepository.UpdateAsync(eventEntity);

        return true;
    }

    private static void ValidateEventDates(
        DateTime startDateTime,
        DateTime endDateTime)
    {
        if (startDateTime >= endDateTime)
            throw new ArgumentException(
                "Event start date and time must be before the end date and time.");
    }

    private static void ValidatePaymentDetails(
        bool isPaid,
        decimal? feeAmount)
    {
        if (isPaid && (!feeAmount.HasValue || feeAmount.Value <= 0))
            throw new ArgumentException(
                "Paid events must have a fee greater than zero.");

        if (!isPaid && feeAmount.HasValue && feeAmount.Value > 0)
            throw new ArgumentException(
                "Free events cannot have a fee amount.");
    }

    private static void ValidateHoldDuration(int holdDurationMinutes)
    {
        if (holdDurationMinutes <= 0)
            throw new ArgumentException(
                "Hold duration must be greater than zero.");
    }

    private static EventDto MapToDto(Event eventEntity)
    {
        return new EventDto
        {
            EventId = eventEntity.EventId,
            VenueId = eventEntity.VenueId,
            EventName = eventEntity.EventName,
            Description = eventEntity.Description,
            StartDateTime = eventEntity.StartDateTime,
            EndDateTime = eventEntity.EndDateTime,
            IsPaid = eventEntity.IsPaid,
            UsesReservedSeating = eventEntity.UsesReservedSeating,
            FeeAmount = eventEntity.FeeAmount,
            HoldDurationMinutes = eventEntity.HoldDurationMinutes,
            IsActive = eventEntity.IsActive
        };
    }
}