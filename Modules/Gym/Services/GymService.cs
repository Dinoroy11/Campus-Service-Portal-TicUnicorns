using CampusServicePortal.Modules.Gym.DTOs;
using CampusServicePortal.Modules.Gym.Entities;
using CampusServicePortal.Modules.Gym.Interfaces.Repository;
using CampusServicePortal.Modules.Gym.Interfaces.Service;
using CampusServicePortal.Modules.Notifications.DTOs;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;
using GymEntity = CampusServicePortal.Modules.Gym.Entities.Gym;

namespace CampusServicePortal.Modules.Gym.Services;

public class GymService : IGymService
{
    private const int PaymentHoldMinutes = 15;

    private readonly IGymRepository _gymRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly INotificationService _notificationService;

    public GymService(
        IGymRepository gymRepository,
        IStudentRepository studentRepository,
        INotificationService notificationService)
    {
        _gymRepository = gymRepository;
        _studentRepository = studentRepository;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<GymDto>> GetAllGymsAsync()
    {
        var gyms = await _gymRepository.GetAllGymsAsync();
        return gyms.Select(MapGym);
    }

    public async Task<GymDto?> GetGymByIdAsync(int gymId)
    {
        var gym = await _gymRepository.GetGymByIdAsync(gymId);
        return gym == null ? null : MapGym(gym);
    }

    public async Task<GymDto> CreateGymAsync(CreateGymDto gymDto)
    {
        ValidateGym(gymDto);

        var gym = new GymEntity
        {
            Name = gymDto.Name.Trim(),
            Location = gymDto.Location.Trim(),
            Capacity = gymDto.Capacity,
            Description = gymDto.Description.Trim(),
            IsActive = true
        };

        var created = await _gymRepository.CreateGymAsync(gym);
        return MapGym(created);
    }

    public async Task<GymDto?> UpdateGymAsync(int gymId, CreateGymDto gymDto)
    {
        ValidateGym(gymDto);

        var existing = await _gymRepository.GetGymByIdAsync(gymId);
        if (existing == null)
        {
            return null;
        }

        existing.Name = gymDto.Name.Trim();
        existing.Location = gymDto.Location.Trim();
        existing.Capacity = gymDto.Capacity;
        existing.Description = gymDto.Description.Trim();

        var updated = await _gymRepository.UpdateGymAsync(existing);
        return updated == null ? null : MapGym(updated);
    }

    public Task<bool> DeleteGymAsync(int gymId)
    {
        return _gymRepository.DeleteGymAsync(gymId);
    }

    public async Task<GymSlotDto> CreateSlotAsync(CreateGymSlotDto dto)
    {
        var gym = await _gymRepository.GetGymByIdAsync(dto.GymId)
            ?? throw new KeyNotFoundException("Gym was not found.");

        if (!gym.IsActive)
        {
            throw new InvalidOperationException("The gym is inactive.");
        }

        if (dto.SlotDate.Date < DateTime.UtcNow.Date)
        {
            throw new InvalidOperationException("A gym slot cannot be created in the past.");
        }

        if (dto.EndTime <= dto.StartTime)
        {
            throw new ArgumentException("End time must be later than start time.");
        }

        if (dto.MaxCapacity <= 0 || dto.MaxCapacity > gym.Capacity)
        {
            throw new ArgumentException(
                $"Slot capacity must be between 1 and the gym capacity ({gym.Capacity}).");
        }

        if (!dto.RequiresPayment && dto.FeeAmount != 0)
        {
            throw new ArgumentException("A free slot must have a fee amount of 0.");
        }

        if (dto.RequiresPayment && dto.FeeAmount <= 0)
        {
            throw new ArgumentException("A paid slot must have a fee amount greater than 0.");
        }

        var existingSlots = await _gymRepository.GetSlotsByGymIdAsync(
            dto.GymId,
            dto.SlotDate.Date);

        var overlaps = existingSlots.Any(x =>
            dto.StartTime < x.EndTime &&
            dto.EndTime > x.StartTime);

        if (overlaps)
        {
            throw new InvalidOperationException(
                "This gym already has an overlapping time slot on the selected date.");
        }

        var slot = new GymSlot
        {
            GymId = dto.GymId,
            SlotDate = dto.SlotDate.Date,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            MaxCapacity = dto.MaxCapacity,
            IsAvailable = true,
            RequiresPayment = dto.RequiresPayment,
            FeeAmount = dto.RequiresPayment ? dto.FeeAmount : 0
        };

        await _gymRepository.CreateSlotAsync(slot);
        slot.Gym = gym;

        return await MapSlotAsync(slot);
    }

    public async Task<GymSlotAvailabilityDto> GetAvailabilityAsync(
        int gymId,
        DateTime slotDate)
    {
        var gym = await _gymRepository.GetGymByIdAsync(gymId)
            ?? throw new KeyNotFoundException("Gym was not found.");

        await ExpireOldHeldBookingsForGymAsync(gymId, slotDate.Date);

        var slots = await _gymRepository.GetSlotsByGymIdAsync(
            gymId,
            slotDate.Date);

        var result = new GymSlotAvailabilityDto
        {
            GymId = gym.GymId,
            GymName = gym.Name,
            Date = slotDate.Date
        };

        foreach (var slot in slots)
        {
            result.Slots.Add(await MapSlotAsync(slot));
        }

        return result;
    }

    public async Task<GymBookingDto> CreateBookingAsync(
        int userId,
        CreateGymBookingRequestDto dto)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId)
            ?? throw new UnauthorizedAccessException(
                "An active student profile is required to book the gym.");

        if (!student.IsActive)
        {
            throw new UnauthorizedAccessException("Student account is inactive.");
        }

        var slot = await _gymRepository.GetSlotByIdAsync(dto.SlotId)
            ?? throw new KeyNotFoundException("Gym slot was not found.");

        if (slot.Gym == null || !slot.Gym.IsActive)
        {
            throw new InvalidOperationException("The gym is inactive.");
        }

        if (!slot.IsAvailable)
        {
            throw new InvalidOperationException("This gym slot is unavailable.");
        }

        var slotStart = slot.SlotDate.Date.Add(slot.StartTime);
        if (slotStart <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("This gym slot has already started or passed.");
        }

        await ExpireHeldBookingsAsync(slot.SlotId);

        var existing = await _gymRepository.GetExistingActiveBookingAsync(
            slot.SlotId,
            userId);

        if (existing != null)
        {
            throw new InvalidOperationException(
                "You already have an active booking for this gym slot.");
        }

        var bookings = await _gymRepository.GetBookingsBySlotIdAsync(slot.SlotId);
        var activeCount = bookings.Count(IsCapacityConsumingBooking);

        if (activeCount >= slot.MaxCapacity)
        {
            throw new InvalidOperationException("This gym slot is full.");
        }

        var now = DateTime.UtcNow;
        var booking = new GymBooking
        {
            SlotId = slot.SlotId,
            UserId = userId,
            BookingDate = slot.SlotDate.Date,
            CreatedAt = now
        };

        if (slot.RequiresPayment)
        {
            booking.Status = "Held";
            booking.PaymentStatus = "Pending";
            booking.HeldAt = now;
            booking.ExpiresAt = now.AddMinutes(PaymentHoldMinutes);
        }
        else
        {
            booking.Status = "Confirmed";
            booking.PaymentStatus = "NotRequired";
        }

        await _gymRepository.CreateBookingAsync(booking);
        booking.GymSlot = slot;

        if (!slot.RequiresPayment)
        {
            await CreateBookingNotificationAsync(
                userId,
                booking,
                "Gym Booking Confirmed",
                $"Your booking for {slot.Gym.Name} on {slot.SlotDate:yyyy-MM-dd} " +
                $"from {slot.StartTime:hh\\:mm} to {slot.EndTime:hh\\:mm} is confirmed.");
        }

        return MapBooking(booking);
    }

    public async Task<IEnumerable<GymBookingDto>> GetMyBookingsAsync(int userId)
    {
        _ = await _studentRepository.GetByUserIdAsync(userId)
            ?? throw new UnauthorizedAccessException("Student profile was not found.");

        var bookings = (await _gymRepository.GetBookingsByUserIdAsync(userId)).ToList();

        foreach (var booking in bookings.Where(x => x.Status == "Held"))
        {
            await ExpireBookingIfNeededAsync(booking);
        }

        // Reload so returned values reflect any expiry updates.
        bookings = (await _gymRepository.GetBookingsByUserIdAsync(userId)).ToList();
        return bookings.Select(MapBooking);
    }

    public async Task<GymBookingDto> PayBookingAsync(
        int userId,
        int bookingId,
        GymPaymentDto dto)
    {
        var booking = await _gymRepository.GetBookingForUserAsync(
            bookingId,
            userId)
            ?? throw new KeyNotFoundException("Gym booking was not found.");

        await ExpireBookingIfNeededAsync(booking);

        if (booking.Status == "Expired")
        {
            throw new InvalidOperationException(
                "The payment hold expired. Please create a new booking.");
        }

        if (booking.Status == "Cancelled")
        {
            throw new InvalidOperationException("Cancelled bookings cannot be paid.");
        }

        var slot = booking.GymSlot
            ?? await _gymRepository.GetSlotByIdAsync(booking.SlotId)
            ?? throw new InvalidOperationException("Gym slot data is missing.");

        booking.GymSlot = slot;

        if (!slot.RequiresPayment)
        {
            throw new InvalidOperationException("This gym slot does not require payment.");
        }

        if (booking.PaymentStatus == "Paid")
        {
            throw new InvalidOperationException("This gym booking has already been paid.");
        }

        booking.PaymentStatus = "Paid";
        booking.PaymentReference = string.IsNullOrWhiteSpace(dto.PaymentReference)
            ? $"SIM-GYM-{DateTime.UtcNow:yyyyMMddHHmmss}-{booking.BookingId}"
            : dto.PaymentReference.Trim();
        booking.PaidAt = DateTime.UtcNow;
        booking.Status = "Confirmed";
        booking.ExpiresAt = null;

        await _gymRepository.UpdateBookingAsync(booking);

        await CreateBookingNotificationAsync(
            userId,
            booking,
            "Gym Booking Confirmed",
            $"Your simulated payment of LKR {slot.FeeAmount:0.00} was successful. " +
            $"Your booking for {slot.Gym?.Name ?? "the gym"} on {slot.SlotDate:yyyy-MM-dd} " +
            $"from {slot.StartTime:hh\\:mm} to {slot.EndTime:hh\\:mm} is confirmed.");

        return MapBooking(booking);
    }

    public async Task<GymBookingDto> CancelBookingAsync(
        int userId,
        int bookingId)
    {
        var booking = await _gymRepository.GetBookingForUserAsync(
            bookingId,
            userId)
            ?? throw new KeyNotFoundException("Gym booking was not found.");

        await ExpireBookingIfNeededAsync(booking);

        if (booking.Status == "Cancelled")
        {
            throw new InvalidOperationException("Gym booking is already cancelled.");
        }

        if (booking.Status == "Completed")
        {
            throw new InvalidOperationException("Completed bookings cannot be cancelled.");
        }

        if (booking.Status == "Expired")
        {
            throw new InvalidOperationException("Expired bookings cannot be cancelled.");
        }

        var slot = booking.GymSlot
            ?? await _gymRepository.GetSlotByIdAsync(booking.SlotId)
            ?? throw new InvalidOperationException("Gym slot data is missing.");

        var slotStart = slot.SlotDate.Date.Add(slot.StartTime);
        if (slotStart <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "A gym booking cannot be cancelled after the slot has started.");
        }

        booking.Status = "Cancelled";
        booking.ExpiresAt = null;

        if (booking.PaymentStatus == "Paid")
        {
            // Simulation only: no real money movement.
            booking.PaymentStatus = "Refunded";
        }

        await _gymRepository.UpdateBookingAsync(booking);

        await CreateBookingNotificationAsync(
            userId,
            booking,
            "Gym Booking Cancelled",
            booking.PaymentStatus == "Refunded"
                ? "Your gym booking was cancelled and the simulated payment was marked as refunded."
                : "Your gym booking was cancelled successfully.");

        return MapBooking(booking);
    }

    public async Task<GymBookingDto> CompleteBookingAsync(int bookingId)
    {
        var booking = await _gymRepository.GetBookingByIdAsync(bookingId)
            ?? throw new KeyNotFoundException("Gym booking was not found.");

        if (booking.Status != "Confirmed")
        {
            throw new InvalidOperationException(
                "Only confirmed gym bookings can be completed.");
        }

        var slot = booking.GymSlot
            ?? await _gymRepository.GetSlotByIdAsync(booking.SlotId)
            ?? throw new InvalidOperationException("Gym slot data is missing.");

        var slotEnd = slot.SlotDate.Date.Add(slot.EndTime);
        if (slotEnd > DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "This gym slot has not ended yet.");
        }

        booking.Status = "Completed";
        await _gymRepository.UpdateBookingAsync(booking);

        return MapBooking(booking);
    }

    private async Task ExpireOldHeldBookingsForGymAsync(int gymId, DateTime date)
    {
        var slots = await _gymRepository.GetSlotsByGymIdAsync(gymId, date);
        foreach (var slot in slots)
        {
            await ExpireHeldBookingsAsync(slot.SlotId);
        }
    }

    private async Task ExpireHeldBookingsAsync(int slotId)
    {
        var bookings = await _gymRepository.GetBookingsBySlotIdAsync(slotId);
        foreach (var booking in bookings.Where(x => x.Status == "Held"))
        {
            await ExpireBookingIfNeededAsync(booking);
        }
    }

    private async Task ExpireBookingIfNeededAsync(GymBooking booking)
    {
        if (booking.Status != "Held" ||
            !booking.ExpiresAt.HasValue ||
            booking.ExpiresAt.Value > DateTime.UtcNow)
        {
            return;
        }

        booking.Status = "Expired";
        booking.PaymentStatus = "Expired";
        await _gymRepository.UpdateBookingAsync(booking);
    }

    private static bool IsCapacityConsumingBooking(GymBooking booking)
    {
        if (booking.Status == "Confirmed")
        {
            return true;
        }

        return booking.Status == "Held" &&
               booking.ExpiresAt.HasValue &&
               booking.ExpiresAt.Value > DateTime.UtcNow;
    }

    private async Task<GymSlotDto> MapSlotAsync(GymSlot slot)
    {
        var bookings = await _gymRepository.GetBookingsBySlotIdAsync(slot.SlotId);
        var bookedCount = bookings.Count(IsCapacityConsumingBooking);
        var remaining = Math.Max(0, slot.MaxCapacity - bookedCount);
        var slotStart = slot.SlotDate.Date.Add(slot.StartTime);

        return new GymSlotDto
        {
            SlotId = slot.SlotId,
            GymId = slot.GymId,
            GymName = slot.Gym?.Name ?? string.Empty,
            SlotDate = slot.SlotDate.Date,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            MaxCapacity = slot.MaxCapacity,
            BookedCount = bookedCount,
            RemainingCapacity = remaining,
            IsAvailable = slot.IsAvailable &&
                          slot.Gym?.IsActive == true &&
                          remaining > 0 &&
                          slotStart > DateTime.UtcNow,
            RequiresPayment = slot.RequiresPayment,
            FeeAmount = slot.RequiresPayment ? slot.FeeAmount : 0
        };
    }

    private static GymBookingDto MapBooking(GymBooking booking)
    {
        var slot = booking.GymSlot;

        return new GymBookingDto
        {
            BookingId = booking.BookingId,
            SlotId = booking.SlotId,
            UserId = booking.UserId,
            GymId = slot?.GymId ?? 0,
            GymName = slot?.Gym?.Name ?? string.Empty,
            BookingDate = booking.BookingDate.Date,
            StartTime = slot?.StartTime ?? TimeSpan.Zero,
            EndTime = slot?.EndTime ?? TimeSpan.Zero,
            Status = booking.Status,
            RequiresPayment = slot?.RequiresPayment ?? false,
            FeeAmount = slot?.RequiresPayment == true ? slot.FeeAmount : 0,
            PaymentStatus = booking.PaymentStatus,
            PaymentReference = booking.PaymentReference,
            PaidAt = booking.PaidAt,
            HeldAt = booking.HeldAt,
            ExpiresAt = booking.ExpiresAt,
            CreatedAt = booking.CreatedAt
        };
    }

    private async Task CreateBookingNotificationAsync(
        int userId,
        GymBooking booking,
        string title,
        string message)
    {
        await _notificationService.CreateAsync(
            new NotificationCreateDto
            {
                UserId = userId,
                Title = title,
                Message = message,
                ReferenceType = "GymBooking",
                ReferenceId = booking.BookingId
            });
    }

    private static GymDto MapGym(GymEntity gym)
    {
        return new GymDto
        {
            GymId = gym.GymId,
            Name = gym.Name,
            Location = gym.Location,
            Capacity = gym.Capacity,
            Description = gym.Description,
            IsActive = gym.IsActive
        };
    }

    private static void ValidateGym(CreateGymDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ArgumentException("Gym name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Location))
        {
            throw new ArgumentException("Gym location is required.");
        }

        if (dto.Capacity <= 0)
        {
            throw new ArgumentException("Gym capacity must be greater than 0.");
        }
    }
}
