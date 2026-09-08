using CampusServicePortal.Modules.Labs.DTOs;
using CampusServicePortal.Modules.Labs.Entities;
using CampusServicePortal.Modules.Labs.Enums;
using CampusServicePortal.Modules.Labs.Interfaces.Repository;
using CampusServicePortal.Modules.Labs.Interfaces.Service;


namespace CampusServicePortal.Modules.Labs.Services;

public class LabService : ILabService
{
    private readonly ILabRepository _labRepository;
    private readonly ILabSeatRepository _seatRepository;
    private readonly ILabTimeSlotRepository _timeSlotRepository;
    private readonly ILabBookingRepository _bookingRepository;

    public LabService(
        ILabRepository labRepository,
        ILabSeatRepository seatRepository,
        ILabTimeSlotRepository timeSlotRepository,
        ILabBookingRepository bookingRepository)
    {
        _labRepository = labRepository;
        _seatRepository = seatRepository;
        _timeSlotRepository = timeSlotRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<List<LabDto>> GetAllLabsAsync()
    {
        var labs = await _labRepository.GetAllAsync();

        return labs.Select(MapToDto).ToList();
    }

    public async Task<LabDto?> GetLabByIdAsync(int labId)
    {
        var lab = await _labRepository.GetByIdAsync(labId);

        return lab == null ? null : MapToDto(lab);
    }

    public async Task<LabDto> CreateLabAsync(CreateLabDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.LabName))
            throw new ArgumentException("Lab name is required.");

        if (dto.Capacity <= 0)
            throw new ArgumentException(
                "Lab capacity must be greater than zero.");

        var lab = new Lab
        {
            LabName = dto.LabName.Trim(),
            Capacity = dto.Capacity,
            Description = dto.Description?.Trim() ?? string.Empty,
            IsActive = true
        };

        var created = await _labRepository.CreateAsync(lab);

        return MapToDto(created);
    }

    public async Task UpdateLabAsync(
        int labId,
        UpdateLabDto dto)
    {
        var lab = await _labRepository.GetByIdAsync(labId);

        if (lab == null)
            throw new KeyNotFoundException("Lab not found.");

        if (string.IsNullOrWhiteSpace(dto.LabName))
            throw new ArgumentException("Lab name is required.");

        if (dto.Capacity <= 0)
            throw new ArgumentException(
                "Lab capacity must be greater than zero.");

        lab.LabName = dto.LabName.Trim();
        lab.Capacity = dto.Capacity;
        lab.Description = dto.Description?.Trim() ?? string.Empty;
        lab.IsActive = dto.IsActive;

        await _labRepository.UpdateAsync(lab);
    }

    public async Task<List<LabSeatDto>> GetSeatsByLabIdAsync(
        int labId)
    {
        var exists = await _labRepository.ExistsAsync(labId);

        if (!exists)
            throw new KeyNotFoundException("Lab not found.");

        var seats = await _seatRepository.GetByLabIdAsync(labId);

        return seats.Select(MapToSeatDto).ToList();
    }

    public async Task<LabSeatDto> CreateSeatAsync(
        CreateLabSeatDto dto)
    {
        var lab = await _labRepository.GetByIdAsync(dto.LabId);

        if (lab == null)
            throw new KeyNotFoundException("Lab not found.");

        if (string.IsNullOrWhiteSpace(dto.SeatNumber))
            throw new ArgumentException(
                "Seat number is required.");

        var existingSeats =
            await _seatRepository.GetByLabIdAsync(dto.LabId);

        if (existingSeats.Count >= lab.Capacity)
            throw new InvalidOperationException(
                "Lab seat capacity has been reached.");

        if (existingSeats.Any(x =>
            x.SeatNumber.Equals(
                dto.SeatNumber.Trim(),
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "Seat number already exists in this lab.");
        }

        var seat = new LabSeat
        {
            LabId = dto.LabId,
            SeatNumber = dto.SeatNumber.Trim(),
            Status = LabSeatStatus.Available,
            IsActive = true
        };

        var created = await _seatRepository.CreateAsync(seat);

        return MapToSeatDto(created);
    }

    public async Task<List<LabTimeSlotDto>>
        GetTimeSlotsByLabIdAsync(int labId)
    {
        var exists = await _labRepository.ExistsAsync(labId);

        if (!exists)
            throw new KeyNotFoundException("Lab not found.");

        var slots =
            await _timeSlotRepository.GetByLabIdAsync(labId);

        return slots.Select(MapToTimeSlotDto).ToList();
    }

    public async Task<LabTimeSlotDto> CreateTimeSlotAsync(
        CreateLabTimeSlotDto dto)
    {
        var lab = await _labRepository.GetByIdAsync(dto.LabId);

        if (lab == null)
            throw new KeyNotFoundException("Lab not found.");

        if (dto.StartTime >= dto.EndTime)
            throw new ArgumentException(
                "Start time must be earlier than end time.");

        var existingSlots =
            await _timeSlotRepository.GetByLabIdAsync(dto.LabId);

        var overlaps = existingSlots.Any(x =>
            dto.StartTime < x.EndTime &&
            dto.EndTime > x.StartTime &&
            x.IsActive);

        if (overlaps)
            throw new InvalidOperationException(
                "The selected time slot overlaps an existing slot.");

        var slot = new LabTimeSlot
        {
            LabId = dto.LabId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsActive = true
        };

        var created =
            await _timeSlotRepository.CreateAsync(slot);

        return MapToTimeSlotDto(created);
    }



    public async Task<LabAvailabilityDto> GetAvailabilityAsync(
    int labId,
    int timeSlotId,
    DateTime bookingDate)
    {
        var lab = await _labRepository.GetByIdAsync(labId);

        if (lab == null)
            throw new KeyNotFoundException("Lab not found.");

        var timeSlot =
            await _timeSlotRepository.GetByIdAsync(timeSlotId);

        if (timeSlot == null ||
            timeSlot.LabId != labId ||
            !timeSlot.IsActive)
        {
            throw new KeyNotFoundException(
                "Valid time slot not found for this lab.");
        }

        var bookings =
            await _bookingRepository.GetByTimeSlotAndDateAsync(
                labId,
                timeSlotId,
                bookingDate);

        var activeBookings = bookings
            .Where(x => x.Status == LabBookingStatus.Booked)
            .ToList();

        var seats =
            await _seatRepository.GetByLabIdAsync(labId);

        var activeSeats = seats
            .Where(x =>
                x.IsActive &&
                x.Status == LabSeatStatus.Available)
            .ToList();

        var bookedSeatIds = activeBookings
            .Where(x => x.LabSeatId.HasValue)
            .Select(x => x.LabSeatId!.Value)
            .ToHashSet();

        var seatAvailability = activeSeats
            .Select(seat => new LabSeatAvailabilityDto
            {
                LabSeatId = seat.LabSeatId,
                SeatNumber = seat.SeatNumber,
                IsAvailable =
                    !bookedSeatIds.Contains(seat.LabSeatId),
                Status =
                    bookedSeatIds.Contains(seat.LabSeatId)
                        ? "Booked"
                        : "Available"
            })
            .ToList();

        var bookedCount = activeBookings.Count;

        var availableCount =
            Math.Max(lab.Capacity - bookedCount, 0);

        return new LabAvailabilityDto
        {
            LabId = lab.LabId,
            LabName = lab.LabName,
            Capacity = lab.Capacity,
            BookedCount = bookedCount,
            AvailableCount = availableCount,
            IsFull = availableCount == 0,
            IsSeatBased = activeSeats.Count > 0,
            Seats = seatAvailability
        };
    }

    public async Task<LabBookingDto> CreateBookingAsync(
    CreateLabBookingDto dto)
    {
        var lab = await _labRepository.GetByIdAsync(dto.LabId);

        if (lab == null)
            throw new KeyNotFoundException("Lab not found.");

        if (!lab.IsActive)
            throw new InvalidOperationException(
                "This lab is currently inactive.");

        var timeSlot =
            await _timeSlotRepository.GetByIdAsync(
                dto.TimeSlotId);

        if (timeSlot == null ||
            !timeSlot.IsActive ||
            timeSlot.LabId != dto.LabId)
        {
            throw new InvalidOperationException(
                "Invalid time slot for this lab.");
        }

        var bookings =
            await _bookingRepository.GetByTimeSlotAndDateAsync(
                dto.LabId,
                dto.TimeSlotId,
                dto.BookingDate);

        var activeBookings = bookings
            .Where(x => x.Status == LabBookingStatus.Booked)
            .ToList();

        if (activeBookings.Count >= lab.Capacity)
            throw new InvalidOperationException(
                "Lab capacity is full for this time slot.");

        var seats =
            await _seatRepository.GetByLabIdAsync(dto.LabId);

        var activeSeats = seats
            .Where(x =>
                x.IsActive &&
                x.Status == LabSeatStatus.Available)
            .ToList();

        int? selectedSeatId = null;

        // Seat-based lab
        if (activeSeats.Count > 0)
        {
            var availableSeat = activeSeats
                .FirstOrDefault(seat =>
                    !activeBookings.Any(booking =>
                        booking.LabSeatId ==
                        seat.LabSeatId));

            if (availableSeat == null)
                throw new InvalidOperationException(
                    "No seats are available for this time slot.");

            selectedSeatId = availableSeat.LabSeatId;
        }

        var booking = new LabBooking
        {
            LabId = dto.LabId,
            TimeSlotId = dto.TimeSlotId,
            LabSeatId = selectedSeatId,
            StudentId = dto.StudentId,
            BookingDate = dto.BookingDate.Date,
            StartTime = timeSlot.StartTime,
            EndTime = timeSlot.EndTime,
            Status = LabBookingStatus.Booked,
            CreatedAt = DateTime.UtcNow
        };

        var created =
            await _bookingRepository.CreateAsync(booking);

        return MapToBookingDto(created);
    }

    public async Task<LabBookingDto?> GetBookingByIdAsync(
    int labBookingId)
    {
        var booking =
            await _bookingRepository.GetByIdAsync(
                labBookingId);

        return booking == null
            ? null
            : MapToBookingDto(booking);
    }

    public async Task CancelBookingAsync(
        int labBookingId)
    {
        var booking =
            await _bookingRepository.GetByIdAsync(
                labBookingId);

        if (booking == null)
            throw new KeyNotFoundException(
                "Booking not found.");

        if (booking.Status != LabBookingStatus.Booked)
            throw new InvalidOperationException(
                "Only active bookings can be cancelled.");

        booking.Status = LabBookingStatus.Cancelled;

        await _bookingRepository.UpdateAsync(booking);
    }

    private static LabDto MapToDto(Lab lab)
    {
        return new LabDto
        {
            LabId = lab.LabId,
            LabName = lab.LabName,
            Capacity = lab.Capacity,
            Description = lab.Description,
            IsActive = lab.IsActive
        };
    }

    private static LabSeatDto MapToSeatDto(
        LabSeat seat)
    {
        return new LabSeatDto
        {
            LabSeatId = seat.LabSeatId,
            LabId = seat.LabId,
            SeatNumber = seat.SeatNumber,
            Status = seat.Status.ToString(),
            IsActive = seat.IsActive
        };
    }

    private static LabTimeSlotDto MapToTimeSlotDto(
        LabTimeSlot slot)
    {
        return new LabTimeSlotDto
        {
            TimeSlotId = slot.TimeSlotId,
            LabId = slot.LabId,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            IsActive = slot.IsActive
        };
    }

    private static LabBookingDto MapToBookingDto(
        LabBooking booking)
    {
        return new LabBookingDto
        {
            LabBookingId = booking.LabBookingId,
            LabId = booking.LabId,
            TimeSlotId = booking.TimeSlotId,
            LabSeatId = booking.LabSeatId,
            StudentId = booking.StudentId,
            BookingDate = booking.BookingDate,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Status = booking.Status.ToString(),
            CreatedAt = booking.CreatedAt
        };
    }
}