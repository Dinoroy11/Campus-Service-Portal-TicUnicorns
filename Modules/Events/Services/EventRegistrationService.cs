using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Entities;
using CampusServicePortal.Modules.Events.Repositories;
using CampusServicePortal_TicUnicorns.Modules.Events.Interfaces.Service;

namespace CampusServicePortal.Modules.Events.Services;

public class EventRegistrationService : IEventRegistrationService
{
    private const int MaxActiveRegistrationsPerStudent = 5;
    private const int NormalHoldMinutes = 15;
    private const int NearEventHoldMinutes = 2;
    private const int NearEventThresholdHours = 2;

    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IEventSeatRepository _seatRepository;
    private readonly IVenueRepository _venueRepository;

    public EventRegistrationService(
        IEventRegistrationRepository registrationRepository,
        IEventRepository eventRepository,
        IEventSeatRepository seatRepository,
        IVenueRepository venueRepository)
    {
        _registrationRepository = registrationRepository;
        _eventRepository = eventRepository;
        _seatRepository = seatRepository;
        _venueRepository = venueRepository;
    }

    public async Task<List<EventRegistrationDto>> GetAllAsync()
    {
        var registrations = await _registrationRepository.GetAllAsync();

        return registrations
            .Select(MapToDto)
            .ToList();
    }

    public async Task<EventRegistrationDto?> GetByIdAsync(
        int eventRegistrationId)
    {
        var registration =
            await _registrationRepository.GetByIdAsync(eventRegistrationId);

        if (registration == null)
            return null;

        return MapToDto(registration);
    }

    public async Task<List<EventRegistrationDto>> GetByEventIdAsync(
        int eventId)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);

        if (eventEntity == null)
            throw new ArgumentException("Event not found.");

        await ReleaseExpiredHoldsAsync(eventId);

        var registrations =
            await _registrationRepository.GetByEventIdAsync(eventId);

        return registrations
            .Select(MapToDto)
            .ToList();
    }

    public async Task<List<EventRegistrationDto>> GetByStudentIdAsync(
        int studentId)
    {
        var registrations =
            await _registrationRepository.GetByStudentIdAsync(studentId);

        await ReleaseExpiredHoldsAsync(registrations);

        return registrations
            .Select(MapToDto)
            .ToList();
    }

    public async Task<EventRegistrationDto> RegisterAsync(
        EventRegistrationDto dto)
    {
        var eventEntity =
            await _eventRepository.GetByIdAsync(dto.EventId);

        if (eventEntity == null)
            throw new ArgumentException("Event not found.");

        if (!eventEntity.IsActive)
            throw new ArgumentException("Event is not active.");

        var now = DateTime.UtcNow;

        if (now >= eventEntity.EndDateTime)
            throw new ArgumentException(
                "Registration is closed because the event has ended.");

        if (now >= eventEntity.StartDateTime)
            throw new ArgumentException(
                "Registration is closed because the event has started.");

        var studentRegistrations =
            await _registrationRepository.GetByStudentIdAsync(
                dto.StudentId);

        await ReleaseExpiredHoldsAsync(studentRegistrations);

        studentRegistrations =
            await _registrationRepository.GetByStudentIdAsync(
                dto.StudentId);

        var activeRegistrations = studentRegistrations
            .Where(IsActiveRegistration)
            .ToList();

        if (activeRegistrations.Count >= MaxActiveRegistrationsPerStudent)
        {
            throw new ArgumentException(
                $"A student can have a maximum of " +
                $"{MaxActiveRegistrationsPerStudent} active event registrations.");
        }

        var duplicateRegistration = activeRegistrations.Any(r =>
            r.EventId == dto.EventId);

        if (duplicateRegistration)
        {
            throw new ArgumentException(
                "Student is already registered for this event.");
        }

        if (eventEntity.UsesReservedSeating)
        {
            return await RegisterWithSeatAsync(
                dto,
                eventEntity,
                now);
        }

        return await RegisterWithoutSeatAsync(
            dto,
            eventEntity,
            now);
    }

    private async Task<EventRegistrationDto> RegisterWithSeatAsync(
        EventRegistrationDto dto,
        Event eventEntity,
        DateTime now)
    {
        if (!dto.EventSeatId.HasValue)
        {
            throw new ArgumentException(
                "A seat must be selected for a reserved seating event.");
        }

        var seat =
            await _seatRepository.GetByIdAsync(dto.EventSeatId.Value);

        if (seat == null)
            throw new ArgumentException("Selected seat not found.");

        if (seat.EventId != eventEntity.EventId)
        {
            throw new ArgumentException(
                "Selected seat does not belong to this event.");
        }

        await ReleaseExpiredHoldsAsync(eventEntity.EventId);

        var eventRegistrations =
            await _registrationRepository.GetByEventIdAsync(
                eventEntity.EventId);

        var seatTaken = eventRegistrations.Any(r =>
            r.EventSeatId == seat.EventSeatId &&
            IsActiveRegistration(r));

        if (seatTaken)
        {
            throw new ArgumentException(
                "Selected seat is currently held or already registered.");
        }

        var holdMinutes = CalculateHoldMinutes(
            eventEntity.StartDateTime,
            now);

        var registration = new EventRegistration
        {
            EventId = eventEntity.EventId,
            StudentId = dto.StudentId,
            EventSeatId = seat.EventSeatId,
            Status = "Held",
            HeldAt = now,
            ExpiresAt = now.AddMinutes(holdMinutes),
            RegisteredAt = now
        };

        var created =
            await _registrationRepository.CreateAsync(registration);

        return MapToDto(created);
    }

    private async Task<EventRegistrationDto> RegisterWithoutSeatAsync(
        EventRegistrationDto dto,
        Event eventEntity,
        DateTime now)
    {
        await ReleaseExpiredHoldsAsync(eventEntity.EventId);

        var registrations =
            await _registrationRepository.GetByEventIdAsync(
                eventEntity.EventId);

        var activeCount = registrations.Count(IsActiveRegistration);

        var venueCapacity = await GetVenueCapacityAsync(
            eventEntity.VenueId);

        if (activeCount >= venueCapacity)
        {
            throw new ArgumentException(
                "This event has reached its capacity.");
        }

        var registration = new EventRegistration
        {
            EventId = eventEntity.EventId,
            StudentId = dto.StudentId,
            EventSeatId = null,
            Status = "Registered",
            HeldAt = null,
            ExpiresAt = null,
            RegisteredAt = now
        };

        var created =
            await _registrationRepository.CreateAsync(registration);

        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(
        int eventRegistrationId,
        EventRegistrationDto dto)
    {
        var registration =
            await _registrationRepository.GetByIdAsync(
                eventRegistrationId);

        if (registration == null)
            return false;

        registration.Status = dto.Status;
        registration.HeldAt = dto.HeldAt;
        registration.ExpiresAt = dto.ExpiresAt;

        await _registrationRepository.UpdateAsync(registration);

        return true;
    }

    private async Task<int> GetVenueCapacityAsync(int venueId)
    {
        var capacity = await _venueRepository.GetCapacityAsync(venueId);

        if (!capacity.HasValue || capacity.Value <= 0)
            throw new ArgumentException(
                "Venue capacity is not available.");

        return capacity.Value;
    }

    private static int CalculateHoldMinutes(
        DateTime eventStartTime,
        DateTime now)
    {
        var timeUntilEvent = eventStartTime - now;

        if (timeUntilEvent <= TimeSpan.FromHours(
                NearEventThresholdHours))
        {
            return NearEventHoldMinutes;
        }

        return NormalHoldMinutes;
    }

    private async Task ReleaseExpiredHoldsAsync(int eventId)
    {
        var registrations =
            await _registrationRepository.GetByEventIdAsync(eventId);

        await ReleaseExpiredHoldsAsync(registrations);
    }

    private async Task ReleaseExpiredHoldsAsync(
        List<EventRegistration> registrations)
    {
        var now = DateTime.UtcNow;

        foreach (var registration in registrations)
        {
            if (registration.Status == "Held" &&
                registration.ExpiresAt.HasValue &&
                registration.ExpiresAt.Value <= now)
            {
                registration.Status = "Expired";
                registration.ExpiresAt = null;

                await _registrationRepository.UpdateAsync(
                    registration);
            }
        }
    }

    private static bool IsActiveRegistration(
        EventRegistration registration)
    {
        if (registration.Status == "Cancelled" ||
            registration.Status == "Expired")
        {
            return false;
        }

        if (registration.Status == "Held" &&
            registration.ExpiresAt.HasValue &&
            registration.ExpiresAt.Value <= DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }

    private static EventRegistrationDto MapToDto(
        EventRegistration registration)
    {
        return new EventRegistrationDto
        {
            EventRegistrationId = registration.EventRegistrationId,
            EventId = registration.EventId,
            StudentId = registration.StudentId,
            EventSeatId = registration.EventSeatId,
            Status = registration.Status,
            HeldAt = registration.HeldAt,
            ExpiresAt = registration.ExpiresAt,
            RegisteredAt = registration.RegisteredAt
        };
    }
}