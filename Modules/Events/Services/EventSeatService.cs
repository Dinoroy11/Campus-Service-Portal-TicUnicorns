using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Entities;
using CampusServicePortal.Modules.Events.Enums;
using CampusServicePortal.Modules.Events.Repositories;

namespace CampusServicePortal.Modules.Events.Services;

public class EventSeatService : IEventSeatService
{
    private static readonly string[] AdminStatuses =
    {
        "Available",
        "Maintenance",
        "Inactive"
    };

    private readonly IEventSeatRepository _eventSeatRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IEventRegistrationRepository _registrationRepository;

    public EventSeatService(
        IEventSeatRepository eventSeatRepository,
        IEventRepository eventRepository,
        IVenueRepository venueRepository,
        IEventRegistrationRepository registrationRepository)
    {
        _eventSeatRepository = eventSeatRepository;
        _eventRepository = eventRepository;
        _venueRepository = venueRepository;
        _registrationRepository = registrationRepository;
    }

    public async Task<List<EventSeatDto>> GetAllAsync()
    {
        var seats = await _eventSeatRepository.GetAllAsync();
        return seats.Select(MapToDto).ToList();
    }

    public async Task<List<EventSeatDto>> GetByEventIdAsync(int eventId)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);
        if (eventEntity == null)
            throw new ArgumentException("Event not found.");

        await ReleaseExpiredHoldsAsync(eventId);
        var seats = await _eventSeatRepository.GetByEventIdAsync(eventId);
        return seats.Select(MapToDto).ToList();
    }

    public async Task<EventSeatDto?> GetByIdAsync(int eventSeatId)
    {
        var seat = await _eventSeatRepository.GetByIdAsync(eventSeatId);
        return seat == null ? null : MapToDto(seat);
    }

    public async Task<EventSeatDto> CreateAsync(EventSeatDto dto)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(dto.EventId);
        if (eventEntity == null)
            throw new ArgumentException("Event not found.");

        if (!eventEntity.UsesReservedSeating)
            throw new ArgumentException("Seats cannot be created for a non-seat event.");

        ValidateSeat(dto);

        var existingSeats = await _eventSeatRepository.GetByEventIdAsync(dto.EventId);
        var venueCapacity = await _venueRepository.GetCapacityAsync(eventEntity.VenueId);

        if (!venueCapacity.HasValue || venueCapacity.Value <= 0)
            throw new ArgumentException("Venue capacity is not available.");

        if (existingSeats.Count >= venueCapacity.Value)
            throw new ArgumentException("The event already has the maximum number of seats allowed by the venue capacity.");

        if (existingSeats.Any(x =>
            x.SeatNumber.Equals(dto.SeatNumber.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("This seat number already exists for the event.");
        }

        var requestedStatus = string.IsNullOrWhiteSpace(dto.Status)
            ? "Available"
            : dto.Status.Trim();

        ValidateAdminStatus(requestedStatus);

        var seat = new EventSeat
        {
            EventId = dto.EventId,
            SeatNumber = dto.SeatNumber.Trim(),
            RowNumber = dto.RowNumber,
            ColumnNumber = dto.ColumnNumber,
            Status = requestedStatus
        };

        var created = await _eventSeatRepository.CreateAsync(seat);
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(int eventSeatId, EventSeatDto dto)
    {
        var seat = await _eventSeatRepository.GetByIdAsync(eventSeatId);
        if (seat == null)
            return false;

        var eventEntity = await _eventRepository.GetByIdAsync(seat.EventId);
        if (eventEntity == null)
            throw new ArgumentException("Event not found.");

        if (!eventEntity.UsesReservedSeating)
            throw new ArgumentException("Seats cannot be managed for a non-seat event.");

        ValidateSeat(dto);

        var registrations = await _registrationRepository.GetByEventIdAsync(seat.EventId);
        var hasActiveRegistration = registrations.Any(x =>
            x.EventSeatId == eventSeatId &&
            IsActiveRegistration(x));

        if (hasActiveRegistration)
            throw new ArgumentException("A held or booked seat cannot be edited until the registration is released.");

        var existingSeats = await _eventSeatRepository.GetByEventIdAsync(seat.EventId);
        if (existingSeats.Any(x =>
            x.EventSeatId != eventSeatId &&
            x.SeatNumber.Equals(dto.SeatNumber.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("This seat number already exists for the event.");
        }

        var requestedStatus = string.IsNullOrWhiteSpace(dto.Status)
            ? "Available"
            : dto.Status.Trim();

        ValidateAdminStatus(requestedStatus);

        seat.SeatNumber = dto.SeatNumber.Trim();
        seat.RowNumber = dto.RowNumber;
        seat.ColumnNumber = dto.ColumnNumber;
        seat.Status = requestedStatus;

        await _eventSeatRepository.UpdateAsync(seat);
        return true;
    }

    private async Task ReleaseExpiredHoldsAsync(int eventId)
    {
        var registrations = await _registrationRepository.GetByEventIdAsync(eventId);
        var now = DateTime.UtcNow;

        foreach (var registration in registrations)
        {
            if (registration.Status != EventRegistrationStatus.Held ||
                !registration.ExpiresAt.HasValue ||
                registration.ExpiresAt.Value > now)
                continue;

            registration.Status = EventRegistrationStatus.Expired;
            registration.ExpiresAt = null;
            await _registrationRepository.UpdateAsync(registration);

            if (!registration.EventSeatId.HasValue)
                continue;

            var seat = await _eventSeatRepository.GetByIdAsync(registration.EventSeatId.Value);
            if (seat != null &&
                !seat.Status.Equals("Inactive", StringComparison.OrdinalIgnoreCase) &&
                !seat.Status.Equals("Maintenance", StringComparison.OrdinalIgnoreCase))
            {
                seat.Status = "Available";
                await _eventSeatRepository.UpdateAsync(seat);
            }
        }
    }

    private static void ValidateSeat(EventSeatDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.SeatNumber))
            throw new ArgumentException("Seat number is required.");

        if (dto.RowNumber <= 0)
            throw new ArgumentException("Row number must be greater than zero.");

        if (dto.ColumnNumber <= 0)
            throw new ArgumentException("Column number must be greater than zero.");
    }

    private static void ValidateAdminStatus(string status)
    {
        if (!AdminStatuses.Any(x => x.Equals(status, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException(
                "Seat status must be Available, Maintenance, or Inactive. Held and Booked are managed by the system.");
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

    private static EventSeatDto MapToDto(EventSeat seat)
    {
        return new EventSeatDto
        {
            EventSeatId = seat.EventSeatId,
            EventId = seat.EventId,
            SeatNumber = seat.SeatNumber,
            RowNumber = seat.RowNumber,
            ColumnNumber = seat.ColumnNumber,
            Status = seat.Status
        };
    }
}
