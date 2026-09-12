using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Entities;
using CampusServicePortal.Modules.Events.Enums;
using CampusServicePortal.Modules.Events.Repositories;
using CampusServicePortal.Modules.Notifications.DTOs;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Events.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;

namespace CampusServicePortal.Modules.Events.Services;

public class EventRegistrationService : IEventRegistrationService
{
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IEventSeatRepository _seatRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly INotificationService _notificationService;

    public EventRegistrationService(
        IEventRegistrationRepository registrationRepository,
        IEventRepository eventRepository,
        IEventSeatRepository seatRepository,
        IVenueRepository venueRepository,
        IStudentRepository studentRepository,
        INotificationService notificationService)
    {
        _registrationRepository = registrationRepository;
        _eventRepository = eventRepository;
        _seatRepository = seatRepository;
        _venueRepository = venueRepository;
        _studentRepository = studentRepository;
        _notificationService = notificationService;
    }

    public async Task<List<EventRegistrationDto>> GetAllAsync()
    {
        var registrations = await _registrationRepository.GetAllAsync();
        await ReleaseExpiredHoldsAsync(registrations);
        registrations = await _registrationRepository.GetAllAsync();
        return registrations.Select(MapToDto).ToList();
    }

    public async Task<EventRegistrationDto?> GetByIdAsync(int eventRegistrationId)
    {
        var registration = await _registrationRepository.GetByIdAsync(eventRegistrationId);
        if (registration == null)
            return null;

        await ReleaseExpiredHoldAsync(registration);
        registration = await _registrationRepository.GetByIdAsync(eventRegistrationId);
        return registration == null ? null : MapToDto(registration);
    }

    public async Task<List<EventRegistrationDto>> GetByEventIdAsync(int eventId)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);
        if (eventEntity == null)
            throw new ArgumentException("Event not found.");

        await ReleaseExpiredHoldsAsync(eventId);
        var registrations = await _registrationRepository.GetByEventIdAsync(eventId);
        return registrations.Select(MapToDto).ToList();
    }

    public async Task<List<EventRegistrationDto>> GetByStudentIdAsync(int studentId)
    {
        var registrations = await _registrationRepository.GetByStudentIdAsync(studentId);
        await ReleaseExpiredHoldsAsync(registrations);
        registrations = await _registrationRepository.GetByStudentIdAsync(studentId);
        return registrations.Select(MapToDto).ToList();
    }

    public async Task<EventRegistrationDto> RegisterAsync(EventRegistrationDto dto)
    {
        var student = await _studentRepository.GetByIdAsync(dto.StudentId);
        if (student == null || !student.IsActive)
            throw new ArgumentException("Student account is not active.");

        var eventEntity = await _eventRepository.GetByIdAsync(dto.EventId);
        if (eventEntity == null)
            throw new ArgumentException("Event not found.");

        if (!eventEntity.IsActive)
            throw new ArgumentException("Event is not active.");

        var now = DateTime.UtcNow;
        if (now >= eventEntity.StartDateTime)
        {
            throw new ArgumentException(
                "Registration is closed because the event has already started.");
        }

        await ReleaseExpiredHoldsAsync(eventEntity.EventId);

        var existing = await _registrationRepository.GetByEventAndStudentAsync(
            eventEntity.EventId,
            dto.StudentId);

        if (existing != null && IsActiveRegistration(existing))
        {
            throw new ArgumentException(
                "Student is already registered or currently holding a place for this event.");
        }

        if (eventEntity.UsesReservedSeating)
        {
            return await RegisterWithSeatAsync(dto, eventEntity, existing, now);
        }

        return await RegisterWithoutSeatAsync(dto, eventEntity, existing, now);
    }

    private async Task<EventRegistrationDto> RegisterWithSeatAsync(
        EventRegistrationDto dto,
        Event eventEntity,
        EventRegistration? existing,
        DateTime now)
    {
        if (!dto.EventSeatId.HasValue)
            throw new ArgumentException("A seat must be selected for this event.");

        var seat = await _seatRepository.GetByIdAsync(dto.EventSeatId.Value);
        if (seat == null)
            throw new ArgumentException("Selected seat not found.");

        if (seat.EventId != eventEntity.EventId)
            throw new ArgumentException("Selected seat does not belong to this event.");

        if (!seat.Status.Equals("Available", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Selected seat is not currently available.");

        var eventRegistrations = await _registrationRepository.GetByEventIdAsync(eventEntity.EventId);
        var activeCount = eventRegistrations.Count(IsActiveRegistration);
        var venueCapacity = await GetVenueCapacityAsync(eventEntity.VenueId);

        if (activeCount >= venueCapacity)
            throw new ArgumentException("This event has reached its venue capacity.");

        var seatTaken = eventRegistrations.Any(x =>
            x.EventSeatId == seat.EventSeatId &&
            IsActiveRegistration(x));

        if (seatTaken)
            throw new ArgumentException("Selected seat is already held or booked.");

        if (existing?.EventSeatId is int oldSeatId && oldSeatId != seat.EventSeatId)
            await ReleaseSeatAsync(oldSeatId);

        var expiresAt = CalculateExpiry(eventEntity, now);
        var registration = existing ?? new EventRegistration();

        registration.EventId = eventEntity.EventId;
        registration.StudentId = dto.StudentId;
        registration.EventSeatId = seat.EventSeatId;
        registration.Status = EventRegistrationStatus.Held;
        registration.HeldAt = now;
        registration.ExpiresAt = expiresAt;
        registration.RegisteredAt = now;

        EventRegistration saved;
        if (existing == null)
            saved = await _registrationRepository.CreateAsync(registration);
        else
        {
            await _registrationRepository.UpdateAsync(registration);
            saved = registration;
        }

        seat.Status = "Held";
        await _seatRepository.UpdateAsync(seat);

        return MapToDto(saved);
    }

    private async Task<EventRegistrationDto> RegisterWithoutSeatAsync(
        EventRegistrationDto dto,
        Event eventEntity,
        EventRegistration? existing,
        DateTime now)
    {
        var registrations = await _registrationRepository.GetByEventIdAsync(eventEntity.EventId);
        var activeCount = registrations.Count(IsActiveRegistration);
        var venueCapacity = await GetVenueCapacityAsync(eventEntity.VenueId);

        if (activeCount >= venueCapacity)
            throw new ArgumentException("This event has reached its venue capacity.");

        if (existing?.EventSeatId is int oldSeatId)
            await ReleaseSeatAsync(oldSeatId);

        var registration = existing ?? new EventRegistration();
        registration.EventId = eventEntity.EventId;
        registration.StudentId = dto.StudentId;
        registration.EventSeatId = null;
        registration.RegisteredAt = now;

        if (eventEntity.IsPaid)
        {
            registration.Status = EventRegistrationStatus.Held;
            registration.HeldAt = now;
            registration.ExpiresAt = CalculateExpiry(eventEntity, now);
        }
        else
        {
            registration.Status = EventRegistrationStatus.Confirmed;
            registration.HeldAt = null;
            registration.ExpiresAt = null;
        }

        EventRegistration saved;
        if (existing == null)
            saved = await _registrationRepository.CreateAsync(registration);
        else
        {
            await _registrationRepository.UpdateAsync(registration);
            saved = registration;
        }

        if (!eventEntity.IsPaid)
            await SendConfirmedNotificationAsync(saved, eventEntity);

        return MapToDto(saved);
    }

    public async Task<EventRegistrationDto> ConfirmFreeRegistrationAsync(
        int eventRegistrationId,
        int studentId)
    {
        var registration = await _registrationRepository.GetByIdAsync(eventRegistrationId);
        if (registration == null)
            throw new ArgumentException("Event registration not found.");

        if (registration.StudentId != studentId)
            throw new UnauthorizedAccessException("This registration does not belong to the current student.");

        await ReleaseExpiredHoldAsync(registration);
        registration = await _registrationRepository.GetByIdAsync(eventRegistrationId)
            ?? throw new ArgumentException("Event registration not found.");

        if (registration.Status == EventRegistrationStatus.Expired)
            throw new ArgumentException("The registration hold has expired. Please select the event again.");

        if (registration.Status != EventRegistrationStatus.Held)
            throw new ArgumentException("Only a held registration can be confirmed.");

        var eventEntity = await _eventRepository.GetByIdAsync(registration.EventId)
            ?? throw new ArgumentException("Event not found.");

        if (eventEntity.IsPaid)
            throw new ArgumentException("This is a paid event. Complete the simulated payment to confirm the registration.");

        if (DateTime.UtcNow >= eventEntity.StartDateTime)
            throw new ArgumentException("The event has already started.");

        registration.Status = EventRegistrationStatus.Confirmed;
        registration.ExpiresAt = null;
        await _registrationRepository.UpdateAsync(registration);

        if (registration.EventSeatId.HasValue)
        {
            var seat = await _seatRepository.GetByIdAsync(registration.EventSeatId.Value);
            if (seat != null)
            {
                seat.Status = "Booked";
                await _seatRepository.UpdateAsync(seat);
            }
        }

        await SendConfirmedNotificationAsync(registration, eventEntity);
        return MapToDto(registration);
    }

    public async Task<EventRegistrationDto> CancelAsync(
        int eventRegistrationId,
        int studentId)
    {
        var registration = await _registrationRepository.GetByIdAsync(eventRegistrationId);
        if (registration == null)
            throw new ArgumentException("Event registration not found.");

        if (registration.StudentId != studentId)
            throw new UnauthorizedAccessException("This registration does not belong to the current student.");

        var eventEntity = await _eventRepository.GetByIdAsync(registration.EventId)
            ?? throw new ArgumentException("Event not found.");

        if (DateTime.UtcNow >= eventEntity.StartDateTime)
            throw new ArgumentException("A registration cannot be cancelled after the event has started.");

        if (registration.Status == EventRegistrationStatus.Cancelled ||
            registration.Status == EventRegistrationStatus.Expired)
            throw new ArgumentException("This registration is already inactive.");

        if (eventEntity.IsPaid && registration.Status == EventRegistrationStatus.Confirmed)
        {
            throw new ArgumentException(
                "A confirmed paid registration cannot be cancelled here because payment reversal is not implemented.");
        }

        registration.Status = EventRegistrationStatus.Cancelled;
        registration.ExpiresAt = null;
        await _registrationRepository.UpdateAsync(registration);

        if (registration.EventSeatId.HasValue)
            await ReleaseSeatAsync(registration.EventSeatId.Value);

        await SendCancelledNotificationAsync(registration, eventEntity);
        return MapToDto(registration);
    }

    public async Task<bool> UpdateAsync(
        int eventRegistrationId,
        EventRegistrationDto dto)
    {
        var registration = await _registrationRepository.GetByIdAsync(eventRegistrationId);
        if (registration == null)
            return false;

        var oldSeatId = registration.EventSeatId;
        registration.Status = dto.Status;
        registration.HeldAt = dto.HeldAt;
        registration.ExpiresAt = dto.ExpiresAt;
        await _registrationRepository.UpdateAsync(registration);

        if (dto.Status == EventRegistrationStatus.Cancelled ||
            dto.Status == EventRegistrationStatus.Expired)
        {
            if (oldSeatId.HasValue)
                await ReleaseSeatAsync(oldSeatId.Value);
        }

        return true;
    }

    private async Task<int> GetVenueCapacityAsync(int venueId)
    {
        var capacity = await _venueRepository.GetCapacityAsync(venueId);
        if (!capacity.HasValue || capacity.Value <= 0)
            throw new ArgumentException("Venue capacity is not available.");
        return capacity.Value;
    }

    private static DateTime CalculateExpiry(Event eventEntity, DateTime now)
    {
        var minutes = eventEntity.HoldDurationMinutes > 0
            ? eventEntity.HoldDurationMinutes
            : 15;

        var requestedExpiry = now.AddMinutes(minutes);
        return requestedExpiry < eventEntity.StartDateTime
            ? requestedExpiry
            : eventEntity.StartDateTime;
    }

    private async Task ReleaseExpiredHoldsAsync(int eventId)
    {
        var registrations = await _registrationRepository.GetByEventIdAsync(eventId);
        await ReleaseExpiredHoldsAsync(registrations);
    }

    private async Task ReleaseExpiredHoldsAsync(List<EventRegistration> registrations)
    {
        foreach (var registration in registrations)
            await ReleaseExpiredHoldAsync(registration);
    }

    private async Task ReleaseExpiredHoldAsync(EventRegistration registration)
    {
        if (registration.Status != EventRegistrationStatus.Held ||
            !registration.ExpiresAt.HasValue ||
            registration.ExpiresAt.Value > DateTime.UtcNow)
            return;

        registration.Status = EventRegistrationStatus.Expired;
        registration.ExpiresAt = null;
        await _registrationRepository.UpdateAsync(registration);

        if (registration.EventSeatId.HasValue)
            await ReleaseSeatAsync(registration.EventSeatId.Value);
    }

    private async Task ReleaseSeatAsync(int eventSeatId)
    {
        var seat = await _seatRepository.GetByIdAsync(eventSeatId);
        if (seat == null)
            return;

        if (!seat.Status.Equals("Inactive", StringComparison.OrdinalIgnoreCase) &&
            !seat.Status.Equals("Maintenance", StringComparison.OrdinalIgnoreCase))
        {
            seat.Status = "Available";
            await _seatRepository.UpdateAsync(seat);
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

    private async Task SendConfirmedNotificationAsync(
        EventRegistration registration,
        Event eventEntity)
    {
        var student = await _studentRepository.GetByIdAsync(registration.StudentId);
        if (student?.UserId is not int userId)
            return;

        await _notificationService.CreateAsync(new NotificationCreateDto
        {
            UserId = userId,
            Title = "Event Registration Confirmed",
            Message = $"Your registration for {eventEntity.EventName} has been confirmed.",
            ReferenceType = "EventRegistration",
            ReferenceId = registration.EventRegistrationId
        });
    }

    private async Task SendCancelledNotificationAsync(
        EventRegistration registration,
        Event eventEntity)
    {
        var student = await _studentRepository.GetByIdAsync(registration.StudentId);
        if (student?.UserId is not int userId)
            return;

        await _notificationService.CreateAsync(new NotificationCreateDto
        {
            UserId = userId,
            Title = "Event Registration Cancelled",
            Message = $"Your registration for {eventEntity.EventName} has been cancelled.",
            ReferenceType = "EventRegistration",
            ReferenceId = registration.EventRegistrationId
        });
    }

    private static EventRegistrationDto MapToDto(EventRegistration registration)
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
