using CampusServicePortal.Modules.Gym.DTOs;

namespace CampusServicePortal.Modules.Gym.Interfaces.Service;

public interface IGymService
{
    Task<IEnumerable<GymDto>> GetAllGymsAsync();
    Task<GymDto?> GetGymByIdAsync(int gymId);
    Task<GymDto> CreateGymAsync(CreateGymDto gymDto);
    Task<GymDto?> UpdateGymAsync(int gymId, CreateGymDto gymDto);
    Task<bool> DeleteGymAsync(int gymId);

    Task<GymSlotDto> CreateSlotAsync(CreateGymSlotDto dto);
    Task<GymSlotAvailabilityDto> GetAvailabilityAsync(int gymId, DateTime slotDate);

    Task<GymBookingDto> CreateBookingAsync(
        int userId,
        CreateGymBookingRequestDto dto);

    Task<IEnumerable<GymBookingDto>> GetMyBookingsAsync(int userId);
    Task<IEnumerable<GymBookingDto>> GetAllBookingsAsync();

    Task<GymBookingDto> PayBookingAsync(
        int userId,
        int bookingId,
        GymPaymentDto dto);

    Task<GymBookingDto> CancelBookingAsync(int userId, int bookingId);
    Task<GymBookingDto> CompleteBookingAsync(int bookingId);
}
