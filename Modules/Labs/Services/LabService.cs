using CampusServicePortal.Modules.Labs.DTOs;
using CampusServicePortal.Modules.Labs.Entities;
using CampusServicePortal.Modules.Labs.Enums;
using CampusServicePortal.Modules.Labs.Interfaces.Repository;
using CampusServicePortal.Modules.Labs.Interfaces.Service;
using CampusServicePortal.Modules.Notifications.DTOs;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;

namespace CampusServicePortal.Modules.Labs.Services;

public class LabService : ILabService
{
    private const double MaxComputerBookingHours = 4.0;

    private readonly ILabRepository _labRepository;
    private readonly ILabSeatRepository _seatRepository;
    private readonly ILabTimeSlotRepository _timeSlotRepository;
    private readonly ILabBookingRepository _bookingRepository;
    private readonly INotificationService _notificationService;
    private readonly IStudentRepository _studentRepository;

    public LabService(
        ILabRepository labRepository,
        ILabSeatRepository seatRepository,
        ILabTimeSlotRepository timeSlotRepository,
        ILabBookingRepository bookingRepository,
        INotificationService notificationService,
        IStudentRepository studentRepository)
    {
        _labRepository = labRepository;
        _seatRepository = seatRepository;
        _timeSlotRepository = timeSlotRepository;
        _bookingRepository = bookingRepository;
        _notificationService = notificationService;
        _studentRepository = studentRepository;
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

        var labType = NormalizeLabType(dto.LabType);

        var lab = new Lab
        {
            LabName = dto.LabName.Trim(),
            LabType = labType,
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

        var labType = NormalizeLabType(dto.LabType);

        if (IsScienceLabType(labType))
        {
            var existingSeats =
                await _seatRepository.GetByLabIdAsync(labId);

            if (existingSeats.Any(x => x.IsActive))
            {
                throw new InvalidOperationException(
                    "A lab with active computer seats cannot be changed to Science. Disable/remove the seats first.");
            }
        }

        lab.LabName = dto.LabName.Trim();
        lab.LabType = labType;
        lab.Capacity = dto.Capacity;
        lab.Description = dto.Description?.Trim() ?? string.Empty;
        lab.IsActive = dto.IsActive;

        await _labRepository.UpdateAsync(lab);
    }

    public async Task<List<LabSeatDto>> GetSeatsByLabIdAsync(
        int labId)
    {
        var lab = await _labRepository.GetByIdAsync(labId);

        if (lab == null)
            throw new KeyNotFoundException("Lab not found.");

        if (!IsComputerLab(lab))
            throw new InvalidOperationException(
                "Science labs do not use individual seat/PC booking.");

        var seats = await _seatRepository.GetByLabIdAsync(labId);

        return seats.Select(MapToSeatDto).ToList();
    }

    public async Task<LabSeatDto> CreateSeatAsync(
        CreateLabSeatDto dto)
    {
        var lab = await _labRepository.GetByIdAsync(dto.LabId);

        if (lab == null)
            throw new KeyNotFoundException("Lab not found.");

        if (!IsComputerLab(lab))
            throw new InvalidOperationException(
                "Seats/PCs can only be created for Computer labs.");

        if (string.IsNullOrWhiteSpace(dto.SeatNumber))
            throw new ArgumentException(
                "Seat number is required.");

        var existingSeats =
            await _seatRepository.GetByLabIdAsync(dto.LabId);

        if (existingSeats.Count(x => x.IsActive) >= lab.Capacity)
            throw new InvalidOperationException(
                "Computer lab seat capacity has been reached.");

        if (existingSeats.Any(x =>
            x.SeatNumber.Equals(
                dto.SeatNumber.Trim(),
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "Seat/PC number already exists in this lab.");
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

    public async Task<LabSeatStatusUpdateResultDto>
        UpdateSeatStatusAsync(
            int labSeatId,
            UpdateLabSeatStatusDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var seat =
            await _seatRepository.GetByIdAsync(labSeatId);

        if (seat == null)
            throw new KeyNotFoundException(
                "Lab PC/seat not found.");

        var lab =
            await _labRepository.GetByIdAsync(seat.LabId);

        if (lab == null)
            throw new KeyNotFoundException(
                "Lab not found.");

        if (!IsComputerLab(lab))
            throw new InvalidOperationException(
                "Individual PC status is only used for Computer labs.");

        var newStatus = NormalizeSeatStatus(dto.Status);
        var oldStatus = seat.Status;

        var result = new LabSeatStatusUpdateResultDto
        {
            LabSeatId = seat.LabSeatId,
            SeatNumber = seat.SeatNumber,
            PreviousStatus = oldStatus.ToString(),
            NewStatus = newStatus.ToString()
        };

        // Available means the PC can be booked again.
        if (newStatus == LabSeatStatus.Available)
        {
            seat.Status = LabSeatStatus.Available;
            seat.IsActive = true;

            await _seatRepository.UpdateAsync(seat);

            return result;
        }

        // Maintenance = temporary block. Inactive = removed from service.
        seat.Status = newStatus;
        seat.IsActive = newStatus != LabSeatStatus.Inactive;

        await _seatRepository.UpdateAsync(seat);

        var affectedBookings =
            await _bookingRepository
                .GetActiveUpcomingBySeatAsync(
                    seat.LabSeatId,
                    DateTime.UtcNow);

        result.AffectedBookings = affectedBookings.Count;

        foreach (var booking in affectedBookings)
        {
            var alternativeSeat =
                await FindAlternativeSeatAsync(
                    lab,
                    seat,
                    booking);

            if (alternativeSeat != null)
            {
                booking.LabSeatId = alternativeSeat.LabSeatId;

                await _bookingRepository.UpdateAsync(booking);

                var notificationSent =
                    await TryNotifyPcReassignedAsync(
                        booking,
                        seat,
                        alternativeSeat,
                        dto.Reason);

                result.ReassignedBookings++;

                result.BookingImpacts.Add(
                    new LabSeatBookingImpactDto
                    {
                        LabBookingId = booking.LabBookingId,
                        StudentId = booking.StudentId,
                        BookingDate = booking.BookingDate,
                        StartTime = booking.StartTime,
                        EndTime = booking.EndTime,
                        OldLabSeatId = seat.LabSeatId,
                        OldSeatNumber = seat.SeatNumber,
                        NewLabSeatId = alternativeSeat.LabSeatId,
                        NewSeatNumber = alternativeSeat.SeatNumber,
                        Action = "Reassigned",
                        NotificationSent = notificationSent
                    });
            }
            else
            {
                booking.Status = LabBookingStatus.Cancelled;

                await _bookingRepository.UpdateAsync(booking);

                var notificationSent =
                    await TryNotifyPcUnavailableAsync(
                        booking,
                        seat,
                        dto.Reason);

                result.CancelledBookings++;

                result.BookingImpacts.Add(
                    new LabSeatBookingImpactDto
                    {
                        LabBookingId = booking.LabBookingId,
                        StudentId = booking.StudentId,
                        BookingDate = booking.BookingDate,
                        StartTime = booking.StartTime,
                        EndTime = booking.EndTime,
                        OldLabSeatId = seat.LabSeatId,
                        OldSeatNumber = seat.SeatNumber,
                        NewLabSeatId = null,
                        NewSeatNumber = null,
                        Action = "Cancelled",
                        NotificationSent = notificationSent
                    });
            }
        }

        return result;
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
        DateTime bookingDate,
        TimeSpan? requestedStartTime = null,
        double? requestedHours = null)
    {
        var lab = await _labRepository.GetByIdAsync(labId);

        if (lab == null)
            throw new KeyNotFoundException("Lab not found.");

        if (!lab.IsActive)
            throw new InvalidOperationException(
                "This lab is currently inactive.");

        var timeSlot =
            await _timeSlotRepository.GetByIdAsync(timeSlotId);

        if (timeSlot == null ||
            timeSlot.LabId != labId ||
            !timeSlot.IsActive)
        {
            throw new KeyNotFoundException(
                "Valid time slot not found for this lab.");
        }

        if (bookingDate.Date < DateTime.UtcNow.Date)
            throw new ArgumentException(
                "Booking date cannot be in the past.");

        if (IsScienceLab(lab))
        {
            return await GetScienceAvailabilityAsync(
                lab,
                timeSlot,
                bookingDate);
        }

        return await GetComputerAvailabilityAsync(
            lab,
            timeSlot,
            bookingDate,
            requestedStartTime,
            requestedHours);
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

        if (dto.BookingDate.Date < DateTime.UtcNow.Date)
            throw new ArgumentException(
                "Booking date cannot be in the past.");

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

        if (IsScienceLab(lab))
        {
            return await CreateScienceBookingAsync(
                lab,
                timeSlot,
                dto);
        }

        return await CreateComputerBookingAsync(
            lab,
            timeSlot,
            dto);
    }

    public async Task<List<LabBookingDto>> GetBookingsByStudentIdAsync(
        int studentId)
    {
        var bookings =
            await _bookingRepository.GetByStudentIdAsync(
                studentId);

        return bookings
            .Select(MapToBookingDto)
            .ToList();
    }

    public async Task<List<LabBookingDto>> GetAllBookingsAsync()
    {
        var bookings = await _bookingRepository.GetAllAsync();

        return bookings
            .Select(MapToBookingDto)
            .ToList();
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

        var student = await _studentRepository.GetByIdAsync(booking.StudentId);

        if (student?.UserId != null)
        {
            await _notificationService.CreateAsync(
                new NotificationCreateDto
                {
                    UserId = student.UserId.Value,
                    Title = "Lab Booking Cancelled",
                    Message =
                        $"Your lab booking on {booking.BookingDate:yyyy-MM-dd} " +
                        $"from {booking.StartTime:hh\\:mm} to {booking.EndTime:hh\\:mm} " +
                        "has been cancelled.",
                    ReferenceType = "LabBooking",
                    ReferenceId = booking.LabBookingId
                });
        }
    }

    // =========================================================
    // SCIENCE LAB
    // =========================================================

    private async Task<LabAvailabilityDto>
        GetScienceAvailabilityAsync(
            Lab lab,
            LabTimeSlot timeSlot,
            DateTime bookingDate)
    {
        var overlappingBookings =
            await _bookingRepository.GetOverlappingAsync(
                lab.LabId,
                bookingDate,
                timeSlot.StartTime,
                timeSlot.EndTime);

        var bookedCount = overlappingBookings.Count;
        var availableCount =
            Math.Max(lab.Capacity - bookedCount, 0);

        return new LabAvailabilityDto
        {
            LabId = lab.LabId,
            LabName = lab.LabName,
            LabType = lab.LabType,
            TimeSlotId = timeSlot.TimeSlotId,
            BookingDate = bookingDate.Date,
            SlotStartTime = timeSlot.StartTime,
            SlotEndTime = timeSlot.EndTime,
            Capacity = lab.Capacity,
            BookedCount = bookedCount,
            AvailableCount = availableCount,
            IsFull = availableCount == 0,
            IsSeatBased = false,
            Message = availableCount == 0
                ? "Science lab capacity is full for this time slot."
                : $"{availableCount} place(s) are available for this science lab time slot.",
            Seats = new List<LabSeatAvailabilityDto>()
        };
    }

    private async Task<LabBookingDto>
        CreateScienceBookingAsync(
            Lab lab,
            LabTimeSlot timeSlot,
            CreateLabBookingDto dto)
    {
        if (dto.LabSeatId.HasValue)
            throw new ArgumentException(
                "Science labs do not require a seat/PC selection.");

        var overlappingBookings =
            await _bookingRepository.GetOverlappingAsync(
                lab.LabId,
                dto.BookingDate,
                timeSlot.StartTime,
                timeSlot.EndTime);

        if (overlappingBookings.Any(x =>
            x.StudentId == dto.StudentId))
        {
            throw new InvalidOperationException(
                "The student already has an overlapping booking in this lab.");
        }

        if (overlappingBookings.Count >= lab.Capacity)
            throw new InvalidOperationException(
                "Science lab capacity is full for this time slot.");

        var booking = new LabBooking
        {
            LabId = dto.LabId,
            TimeSlotId = dto.TimeSlotId,
            LabSeatId = null,
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

    private async Task<LabSeat?> FindAlternativeSeatAsync(
        Lab lab,
        LabSeat blockedSeat,
        LabBooking affectedBooking)
    {
        var seats =
            await _seatRepository.GetByLabIdAsync(lab.LabId);

        var candidates = seats
            .Where(x =>
                x.LabSeatId != blockedSeat.LabSeatId &&
                x.IsActive &&
                x.Status == LabSeatStatus.Available)
            .OrderBy(x => x.SeatNumber)
            .ToList();

        if (candidates.Count == 0)
            return null;

        var dateBookings =
            await _bookingRepository.GetByLabAndDateAsync(
                lab.LabId,
                affectedBooking.BookingDate);

        var activeBookings = dateBookings
            .Where(x =>
                x.Status == LabBookingStatus.Booked &&
                x.LabBookingId != affectedBooking.LabBookingId)
            .ToList();

        foreach (var candidate in candidates)
        {
            var conflict = activeBookings.Any(x =>
                x.LabSeatId == candidate.LabSeatId &&
                Overlaps(
                    x.StartTime,
                    x.EndTime,
                    affectedBooking.StartTime,
                    affectedBooking.EndTime));

            if (!conflict)
                return candidate;
        }

        return null;
    }

    private async Task<bool> TryNotifyPcReassignedAsync(
        LabBooking booking,
        LabSeat oldSeat,
        LabSeat newSeat,
        string? reason)
    {
        var student =
            await _studentRepository.GetByIdAsync(
                booking.StudentId);

        if (student?.UserId == null)
            return false;

        var reasonText = string.IsNullOrWhiteSpace(reason)
            ? "The originally booked PC is temporarily unavailable."
            : reason.Trim();

        try
        {
            await _notificationService.CreateAsync(
                new NotificationCreateDto
                {
                    UserId = student.UserId.Value,
                    Title = "Lab PC Changed",
                    Message =
                        $"PC {oldSeat.SeatNumber} is unavailable. " +
                        $"Your booking on {booking.BookingDate:yyyy-MM-dd} " +
                        $"from {booking.StartTime:hh\\:mm} to {booking.EndTime:hh\\:mm} " +
                        $"has been moved to PC {newSeat.SeatNumber}. " +
                        $"Reason: {reasonText}",
                    ReferenceType = "LabBooking",
                    ReferenceId = booking.LabBookingId
                });

            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> TryNotifyPcUnavailableAsync(
        LabBooking booking,
        LabSeat oldSeat,
        string? reason)
    {
        var student =
            await _studentRepository.GetByIdAsync(
                booking.StudentId);

        if (student?.UserId == null)
            return false;

        var reasonText = string.IsNullOrWhiteSpace(reason)
            ? "The booked PC is unavailable."
            : reason.Trim();

        try
        {
            await _notificationService.CreateAsync(
                new NotificationCreateDto
                {
                    UserId = student.UserId.Value,
                    Title = "Lab Booking Needs Attention",
                    Message =
                        $"PC {oldSeat.SeatNumber} is unavailable for your booking on " +
                        $"{booking.BookingDate:yyyy-MM-dd} from {booking.StartTime:hh\\:mm} " +
                        $"to {booking.EndTime:hh\\:mm}. No other PC is available for the same period, " +
                        $"so this booking was cancelled. Please check lab availability and choose another time. " +
                        $"Reason: {reasonText}",
                    ReferenceType = "LabBooking",
                    ReferenceId = booking.LabBookingId
                });

            return true;
        }
        catch
        {
            return false;
        }
    }

    // =========================================================
    // COMPUTER LAB
    // =========================================================

    private async Task<LabAvailabilityDto>
        GetComputerAvailabilityAsync(
            Lab lab,
            LabTimeSlot timeSlot,
            DateTime bookingDate,
            TimeSpan? requestedStartTime,
            double? requestedHours)
    {
        if (!requestedStartTime.HasValue)
            throw new ArgumentException(
                "Requested start time is required for a Computer lab.");

        if (!requestedHours.HasValue)
            throw new ArgumentException(
                "Requested duration is required for a Computer lab.");

        ValidateComputerDuration(requestedHours.Value);

        var requestedStart = requestedStartTime.Value;
        var requestedEnd = requestedStart
            .Add(TimeSpan.FromHours(requestedHours.Value));

        ValidateRequestedWindow(
            timeSlot,
            requestedStart,
            requestedEnd);

        var allBookings =
            await _bookingRepository.GetByLabAndDateAsync(
                lab.LabId,
                bookingDate);

        var activeBookings = allBookings
            .Where(x => x.Status == LabBookingStatus.Booked)
            .ToList();

        var seats =
            await _seatRepository.GetByLabIdAsync(lab.LabId);

        var activeSeats = seats
            .Where(x =>
                x.IsActive &&
                x.Status == LabSeatStatus.Available)
            .OrderBy(x => x.SeatNumber)
            .ToList();

        var seatAvailability = activeSeats
            .Select(seat => BuildSeatAvailability(
                seat,
                activeBookings,
                timeSlot.StartTime,
                timeSlot.EndTime,
                requestedStart,
                requestedHours.Value))
            .ToList();

        var fullyAvailableCount = seatAvailability
            .Count(x => x.CanBookRequestedDuration);

        string message;

        if (fullyAvailableCount > 0)
        {
            message =
                $"{fullyAvailableCount} PC seat(s) can satisfy the requested {requestedHours.Value:0.##} hour booking.";
        }
        else
        {
            var bestSuggestion = seatAvailability
                .Where(x => x.SuggestedHours > 0)
                .OrderByDescending(x => x.SuggestedHours)
                .ThenBy(x => x.SuggestedStartTime)
                .FirstOrDefault();

            message = bestSuggestion == null
                ? "No PC seat is available for the requested period on this time slot."
                : $"The full {requestedHours.Value:0.##} hours are unavailable. Best current suggestion: PC {bestSuggestion.SeatNumber}, {bestSuggestion.SuggestedHours:0.##} hour(s) from {bestSuggestion.SuggestedStartTime} to {bestSuggestion.SuggestedEndTime}.";
        }

        return new LabAvailabilityDto
        {
            LabId = lab.LabId,
            LabName = lab.LabName,
            LabType = lab.LabType,
            TimeSlotId = timeSlot.TimeSlotId,
            BookingDate = bookingDate.Date,
            SlotStartTime = timeSlot.StartTime,
            SlotEndTime = timeSlot.EndTime,
            Capacity = lab.Capacity,
            BookedCount = activeBookings.Count,
            AvailableCount = fullyAvailableCount,
            IsFull = fullyAvailableCount == 0,
            IsSeatBased = true,
            RequestedStartTime = requestedStart,
            RequestedEndTime = requestedEnd,
            RequestedHours = requestedHours.Value,
            MaxComputerBookingHours = MaxComputerBookingHours,
            Message = message,
            Seats = seatAvailability
        };
    }

    private async Task<LabBookingDto>
        CreateComputerBookingAsync(
            Lab lab,
            LabTimeSlot timeSlot,
            CreateLabBookingDto dto)
    {
        if (!dto.LabSeatId.HasValue)
            throw new ArgumentException(
                "Computer lab booking requires a PC seat selection.");

        if (!dto.RequestedStartTime.HasValue)
            throw new ArgumentException(
                "Requested start time is required for a Computer lab.");

        if (!dto.RequestedHours.HasValue)
            throw new ArgumentException(
                "Requested duration is required for a Computer lab.");

        ValidateComputerDuration(dto.RequestedHours.Value);

        var requestedStart = dto.RequestedStartTime.Value;
        var requestedEnd = requestedStart
            .Add(TimeSpan.FromHours(dto.RequestedHours.Value));

        ValidateRequestedWindow(
            timeSlot,
            requestedStart,
            requestedEnd);

        var seat =
            await _seatRepository.GetByIdAsync(
                dto.LabSeatId.Value);

        if (seat == null ||
            seat.LabId != lab.LabId ||
            !seat.IsActive ||
            seat.Status != LabSeatStatus.Available)
        {
            throw new InvalidOperationException(
                "The selected PC seat is not available for booking.");
        }

        var allBookings =
            await _bookingRepository.GetByLabAndDateAsync(
                lab.LabId,
                dto.BookingDate);

        var activeBookings = allBookings
            .Where(x => x.Status == LabBookingStatus.Booked)
            .ToList();

        var studentConflict = activeBookings.Any(x =>
            x.StudentId == dto.StudentId &&
            Overlaps(
                x.StartTime,
                x.EndTime,
                requestedStart,
                requestedEnd));

        if (studentConflict)
            throw new InvalidOperationException(
                "The student already has an overlapping booking in this lab.");

        var seatBookings = activeBookings
            .Where(x => x.LabSeatId == seat.LabSeatId)
            .ToList();

        var seatConflict = seatBookings.Any(x =>
            Overlaps(
                x.StartTime,
                x.EndTime,
                requestedStart,
                requestedEnd));

        if (seatConflict)
        {
            var suggestion = BuildSeatAvailability(
                seat,
                activeBookings,
                timeSlot.StartTime,
                timeSlot.EndTime,
                requestedStart,
                dto.RequestedHours.Value);

            if (suggestion.SuggestedHours > 0)
            {
                throw new InvalidOperationException(
                    $"PC {seat.SeatNumber} is not available for the full requested period. Suggested availability: {suggestion.SuggestedHours:0.##} hour(s) from {suggestion.SuggestedStartTime} to {suggestion.SuggestedEndTime}.");
            }

            throw new InvalidOperationException(
                $"PC {seat.SeatNumber} is not available for the requested period.");
        }

        var booking = new LabBooking
        {
            LabId = dto.LabId,
            TimeSlotId = dto.TimeSlotId,
            LabSeatId = seat.LabSeatId,
            StudentId = dto.StudentId,
            BookingDate = dto.BookingDate.Date,
            StartTime = requestedStart,
            EndTime = requestedEnd,
            Status = LabBookingStatus.Booked,
            CreatedAt = DateTime.UtcNow
        };

        var created =
            await _bookingRepository.CreateAsync(booking);

        return MapToBookingDto(created);
    }

    private static LabSeatAvailabilityDto BuildSeatAvailability(
        LabSeat seat,
        List<LabBooking> activeBookings,
        TimeSpan slotStart,
        TimeSpan slotEnd,
        TimeSpan requestedStart,
        double requestedHours)
    {
        var seatBookings = activeBookings
            .Where(x => x.LabSeatId == seat.LabSeatId)
            .OrderBy(x => x.StartTime)
            .ToList();

        var requestedDuration =
            TimeSpan.FromHours(requestedHours);

        var requestedEnd =
            requestedStart.Add(requestedDuration);

        var canBookRequested =
            requestedEnd <= slotEnd &&
            !seatBookings.Any(x =>
                Overlaps(
                    x.StartTime,
                    x.EndTime,
                    requestedStart,
                    requestedEnd));

        var freeGaps = BuildFreeGaps(
            seatBookings,
            slotStart,
            slotEnd);

        var gapAtRequestedStart = freeGaps
            .FirstOrDefault(gap =>
                requestedStart >= gap.Start &&
                requestedStart < gap.End);

        var availableFromRequestedStart = 0.0;

        if (gapAtRequestedStart.End > gapAtRequestedStart.Start)
        {
            availableFromRequestedStart =
                Math.Max(
                    0,
                    (gapAtRequestedStart.End - requestedStart)
                        .TotalHours);
        }

        TimeSpan? suggestedStart = null;
        TimeSpan? suggestedEnd = null;
        double suggestedHours = 0;

        if (canBookRequested)
        {
            suggestedStart = requestedStart;
            suggestedEnd = requestedEnd;
            suggestedHours = requestedHours;
        }
        else
        {
            // Prefer a later gap that can satisfy the full request.
            var fullGap = freeGaps
                .Select(gap => new
                {
                    Start = gap.Start < requestedStart
                        ? requestedStart
                        : gap.Start,
                    gap.End
                })
                .Where(gap =>
                    gap.End > gap.Start &&
                    (gap.End - gap.Start).TotalHours >= requestedHours)
                .OrderBy(gap => gap.Start)
                .FirstOrDefault();

            if (fullGap != null)
            {
                suggestedStart = fullGap.Start;
                suggestedEnd = fullGap.Start.Add(requestedDuration);
                suggestedHours = requestedHours;
            }
            else
            {
                // Otherwise suggest the longest continuous free period
                // remaining after the requested start.
                var bestGap = freeGaps
                    .Select(gap => new
                    {
                        Start = gap.Start < requestedStart
                            ? requestedStart
                            : gap.Start,
                        gap.End
                    })
                    .Where(gap => gap.End > gap.Start)
                    .Select(gap => new
                    {
                        gap.Start,
                        gap.End,
                        Hours = (gap.End - gap.Start).TotalHours
                    })
                    .OrderByDescending(gap => gap.Hours)
                    .ThenBy(gap => gap.Start)
                    .FirstOrDefault();

                if (bestGap != null)
                {
                    suggestedStart = bestGap.Start;
                    suggestedEnd = bestGap.End;
                    suggestedHours = bestGap.Hours;
                }
            }
        }

        return new LabSeatAvailabilityDto
        {
            LabSeatId = seat.LabSeatId,
            SeatNumber = seat.SeatNumber,
            IsAvailable = canBookRequested,
            Status = canBookRequested
                ? "Available"
                : suggestedHours > 0
                    ? "PartiallyAvailable"
                    : "Unavailable",
            CanBookRequestedDuration = canBookRequested,
            AvailableHoursFromRequestedStart =
                Math.Round(availableFromRequestedStart, 2),
            SuggestedStartTime = suggestedStart,
            SuggestedEndTime = suggestedEnd,
            SuggestedHours = Math.Round(suggestedHours, 2)
        };
    }

    private static List<(TimeSpan Start, TimeSpan End)> BuildFreeGaps(
        List<LabBooking> bookings,
        TimeSpan slotStart,
        TimeSpan slotEnd)
    {
        var busy = bookings
            .Select(x =>
            {
                var start = x.StartTime < slotStart
                    ? slotStart
                    : x.StartTime;

                var end = x.EndTime > slotEnd
                    ? slotEnd
                    : x.EndTime;

                return (Start: start, End: end);
            })
            .Where(x => x.End > x.Start)
            .OrderBy(x => x.Start)
            .ToList();

        var merged = new List<(TimeSpan Start, TimeSpan End)>();

        foreach (var interval in busy)
        {
            if (merged.Count == 0)
            {
                merged.Add(interval);
                continue;
            }

            var last = merged[^1];

            if (interval.Start <= last.End)
            {
                merged[^1] = (
                    last.Start,
                    interval.End > last.End
                        ? interval.End
                        : last.End);
            }
            else
            {
                merged.Add(interval);
            }
        }

        var free = new List<(TimeSpan Start, TimeSpan End)>();
        var cursor = slotStart;

        foreach (var interval in merged)
        {
            if (interval.Start > cursor)
            {
                free.Add((cursor, interval.Start));
            }

            if (interval.End > cursor)
            {
                cursor = interval.End;
            }
        }

        if (cursor < slotEnd)
        {
            free.Add((cursor, slotEnd));
        }

        return free;
    }

    private static void ValidateComputerDuration(double requestedHours)
    {
        if (requestedHours <= 0)
            throw new ArgumentException(
                "Requested duration must be greater than zero.");

        if (requestedHours > MaxComputerBookingHours)
            throw new ArgumentException(
                "Computer lab booking cannot exceed 4 hours.");
    }

    private static void ValidateRequestedWindow(
        LabTimeSlot timeSlot,
        TimeSpan requestedStart,
        TimeSpan requestedEnd)
    {
        if (requestedStart < timeSlot.StartTime ||
            requestedStart >= timeSlot.EndTime)
        {
            throw new ArgumentException(
                "Requested start time must be inside the selected lab time slot.");
        }

        if (requestedEnd > timeSlot.EndTime)
        {
            var availableHours =
                Math.Max(
                    0,
                    (timeSlot.EndTime - requestedStart)
                        .TotalHours);

            throw new InvalidOperationException(
                $"The requested duration exceeds the selected time slot. From {requestedStart}, only {availableHours:0.##} hour(s) remain in this slot.");
        }
    }

    private static bool Overlaps(
        TimeSpan existingStart,
        TimeSpan existingEnd,
        TimeSpan requestedStart,
        TimeSpan requestedEnd)
    {
        return existingStart < requestedEnd &&
               existingEnd > requestedStart;
    }

    private static LabSeatStatus NormalizeSeatStatus(
        string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException(
                "PC status is required.");

        if (status.Equals(
            "Available",
            StringComparison.OrdinalIgnoreCase))
        {
            return LabSeatStatus.Available;
        }

        if (status.Equals(
            "Maintenance",
            StringComparison.OrdinalIgnoreCase))
        {
            return LabSeatStatus.Maintenance;
        }

        if (status.Equals(
            "Inactive",
            StringComparison.OrdinalIgnoreCase))
        {
            return LabSeatStatus.Inactive;
        }

        throw new ArgumentException(
            "PC status must be Available, Maintenance or Inactive.");
    }

    private static string NormalizeLabType(string? labType)
    {
        if (string.IsNullOrWhiteSpace(labType))
            throw new ArgumentException(
                "Lab type is required.");

        if (labType.Equals(
            "Science",
            StringComparison.OrdinalIgnoreCase))
        {
            return "Science";
        }

        if (labType.Equals(
            "Computer",
            StringComparison.OrdinalIgnoreCase))
        {
            return "Computer";
        }

        throw new ArgumentException(
            "Lab type must be Science or Computer.");
    }

    private static bool IsComputerLab(Lab lab)
    {
        return lab.LabType.Equals(
            "Computer",
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsScienceLab(Lab lab)
    {
        return lab.LabType.Equals(
            "Science",
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsScienceLabType(string labType)
    {
        return labType.Equals(
            "Science",
            StringComparison.OrdinalIgnoreCase);
    }

    private static LabDto MapToDto(Lab lab)
    {
        return new LabDto
        {
            LabId = lab.LabId,
            LabName = lab.LabName,
            LabType = lab.LabType,
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
