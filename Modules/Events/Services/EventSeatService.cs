using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Entities;
using CampusServicePortal.Modules.Events.Repositories;

namespace CampusServicePortal.Modules.Events.Services;

public class EventSeatService : IEventSeatService
{
    private readonly IEventSeatRepository _eventSeatRepository;
    private readonly IEventRepository _eventRepository;

    public EventSeatService(
        IEventSeatRepository eventSeatRepository,
        IEventRepository eventRepository)
    {
        _eventSeatRepository = eventSeatRepository;
        _eventRepository = eventRepository;
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

        var seats = await _eventSeatRepository.GetByEventIdAsync(eventId);

        return seats.Select(MapToDto).ToList();
    }

    public async Task<EventSeatDto?> GetByIdAsync(int eventSeatId)
    {
        var seat = await _eventSeatRepository.GetByIdAsync(eventSeatId);

        if (seat == null)
            return null;

        return MapToDto(seat);
    }

    public async Task<EventSeatDto> CreateAsync(EventSeatDto dto)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(dto.EventId);

        if (eventEntity == null)
            throw new ArgumentException("Event not found.");

        if (!eventEntity.UsesReservedSeating)
            throw new ArgumentException(
                "Seats cannot be created for a non-seat event.");

        if (string.IsNullOrWhiteSpace(dto.SeatNumber))
            throw new ArgumentException("Seat number is required.");

        if (dto.RowNumber < 0)
            throw new ArgumentException("Row number cannot be negative.");

        if (dto.ColumnNumber < 0)
            throw new ArgumentException("Column number cannot be negative.");

        var existingSeats =
            await _eventSeatRepository.GetByEventIdAsync(dto.EventId);

        var duplicateSeat = existingSeats.Any(s =>
            s.SeatNumber.Equals(
                dto.SeatNumber.Trim(),
                StringComparison.OrdinalIgnoreCase));

        if (duplicateSeat)
            throw new ArgumentException(
                "This seat number already exists for the event.");

        var seat = new EventSeat
        {
            EventId = dto.EventId,
            SeatNumber = dto.SeatNumber.Trim(),
            RowNumber = dto.RowNumber,
            ColumnNumber = dto.ColumnNumber,
            Status = string.IsNullOrWhiteSpace(dto.Status)
                ? "Available"
                : dto.Status.Trim()
        };

        var createdSeat = await _eventSeatRepository.CreateAsync(seat);

        return MapToDto(createdSeat);
    }

    public async Task<bool> UpdateAsync(
        int eventSeatId,
        EventSeatDto dto)
    {
        var seat = await _eventSeatRepository.GetByIdAsync(eventSeatId);

        if (seat == null)
            return false;

        var eventEntity = await _eventRepository.GetByIdAsync(dto.EventId);

        if (eventEntity == null)
            throw new ArgumentException("Event not found.");

        if (!eventEntity.UsesReservedSeating)
            throw new ArgumentException(
                "Seats cannot be managed for a non-seat event.");

        if (string.IsNullOrWhiteSpace(dto.SeatNumber))
            throw new ArgumentException("Seat number is required.");

        if (dto.RowNumber < 0)
            throw new ArgumentException(
                "Row number cannot be negative.");

        if (dto.ColumnNumber < 0)
            throw new ArgumentException(
                "Column number cannot be negative.");

        var existingSeats =
            await _eventSeatRepository.GetByEventIdAsync(dto.EventId);

        var duplicateSeat = existingSeats.Any(s =>
            s.EventSeatId != eventSeatId &&
            s.SeatNumber.Equals(
                dto.SeatNumber.Trim(),
                StringComparison.OrdinalIgnoreCase));

        if (duplicateSeat)
            throw new ArgumentException(
                "This seat number already exists for the event.");

        seat.EventId = dto.EventId;
        seat.SeatNumber = dto.SeatNumber.Trim();
        seat.RowNumber = dto.RowNumber;
        seat.ColumnNumber = dto.ColumnNumber;
        seat.Status = dto.Status.Trim();

        await _eventSeatRepository.UpdateAsync(seat);

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