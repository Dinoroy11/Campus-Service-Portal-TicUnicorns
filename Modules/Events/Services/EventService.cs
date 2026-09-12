using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Entities;
using CampusServicePortal.Modules.Events.Enums;
using CampusServicePortal.Modules.Events.Repositories;

namespace CampusServicePortal.Modules.Events.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IEventSeatRepository _seatRepository;
    private readonly IEventRegistrationRepository _registrationRepository;

    public EventService(
        IEventRepository eventRepository,
        IVenueRepository venueRepository,
        IEventSeatRepository seatRepository,
        IEventRegistrationRepository registrationRepository)
    {
        _eventRepository = eventRepository;
        _venueRepository = venueRepository;
        _seatRepository = seatRepository;
        _registrationRepository = registrationRepository;
    }

    public async Task<List<EventDto>> GetAllAsync()
    {
        var events = await _eventRepository.GetAllAsync();
        return events.Select(MapToDto).ToList();
    }

    public async Task<EventDto?> GetByIdAsync(int eventId)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);
        return eventEntity == null ? null : MapToDto(eventEntity);
    }

    public async Task<EventDto> CreateAsync(CreateEventDto dto)
    {
        ValidateEventName(dto.EventName);
        ValidateEventDates(dto.StartDateTime, dto.EndDateTime, true);
        ValidatePaymentDetails(dto.IsPaid, dto.FeeAmount);
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
            Description = dto.Description?.Trim(),
            StartDateTime = dto.StartDateTime,
            EndDateTime = dto.EndDateTime,
            IsPaid = dto.IsPaid,
            UsesReservedSeating = dto.UsesReservedSeating,
            FeeAmount = dto.IsPaid ? dto.FeeAmount : null,
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

        ValidateEventName(dto.EventName);
        ValidateEventDates(dto.StartDateTime, dto.EndDateTime, false);
        ValidatePaymentDetails(dto.IsPaid, dto.FeeAmount);
        ValidateHoldDuration(dto.HoldDurationMinutes);

        var venue = await _venueRepository.GetByIdAsync(dto.VenueId);
        if (venue == null)
            throw new ArgumentException("Venue not found.");

        if (!venue.IsActive)
            throw new ArgumentException("Selected venue is not active.");

        var registrations = await _registrationRepository.GetByEventIdAsync(eventId);
        var activeCount = registrations.Count(IsActiveRegistration);

        if (activeCount > venue.Capacity)
        {
            throw new ArgumentException(
                "The selected venue capacity is smaller than the event's current active registrations.");
        }

        var seats = await _seatRepository.GetByEventIdAsync(eventId);
        if (seats.Count > venue.Capacity)
        {
            throw new ArgumentException(
                "The selected venue capacity is smaller than the number of seats already created for this event.");
        }

        if (eventEntity.UsesReservedSeating &&
            !dto.UsesReservedSeating &&
            registrations.Any(x => x.EventSeatId.HasValue && IsActiveRegistration(x)))
        {
            throw new ArgumentException(
                "Reserved seating cannot be disabled while active seat registrations exist.");
        }

        eventEntity.VenueId = dto.VenueId;
        eventEntity.EventName = dto.EventName.Trim();
        eventEntity.Description = dto.Description?.Trim();
        eventEntity.StartDateTime = dto.StartDateTime;
        eventEntity.EndDateTime = dto.EndDateTime;
        eventEntity.IsPaid = dto.IsPaid;
        eventEntity.UsesReservedSeating = dto.UsesReservedSeating;
        eventEntity.FeeAmount = dto.IsPaid ? dto.FeeAmount : null;
        eventEntity.HoldDurationMinutes = dto.HoldDurationMinutes;
        eventEntity.IsActive = dto.IsActive;

        await _eventRepository.UpdateAsync(eventEntity);
        return true;
    }

    private static void ValidateEventName(string eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName))
            throw new ArgumentException("Event name is required.");
    }

    private static void ValidateEventDates(
        DateTime startDateTime,
        DateTime endDateTime,
        bool isCreate)
    {
        if (startDateTime >= endDateTime)
        {
            throw new ArgumentException(
                "Event start date and time must be before the end date and time.");
        }

        if (isCreate && startDateTime <= DateTime.UtcNow)
        {
            throw new ArgumentException(
                "A new event must start in the future.");
        }
    }

    private static void ValidatePaymentDetails(bool isPaid, decimal? feeAmount)
    {
        if (isPaid && (!feeAmount.HasValue || feeAmount.Value <= 0))
        {
            throw new ArgumentException(
                "Paid events must have a fee greater than zero.");
        }

        if (!isPaid && feeAmount.HasValue && feeAmount.Value > 0)
        {
            throw new ArgumentException(
                "Free events cannot have a fee amount.");
        }
    }

    private static void ValidateHoldDuration(int holdDurationMinutes)
    {
        if (holdDurationMinutes <= 0 || holdDurationMinutes > 60)
        {
            throw new ArgumentException(
                "Hold duration must be between 1 and 60 minutes.");
        }
    }

    private static bool IsActiveRegistration(EventRegistration registration)
    {
        if (registration.Status == EventRegistrationStatus.Cancelled ||
            registration.Status == EventRegistrationStatus.Expired)
            return false;

        if (registration.Status == EventRegistrationStatus.Held &&
            registration.ExpiresAt.HasValue &&
            registration.ExpiresAt.Value <= DateTime.UtcNow)
            return false;

        return true;
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
