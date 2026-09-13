using CampusServicePortal.Modules.Gym.Entities;
using GymEntity = CampusServicePortal.Modules.Gym.Entities.Gym;

namespace CampusServicePortal.Modules.Gym.Interfaces.Repository;

public interface IGymRepository
{
    Task<IEnumerable<GymEntity>> GetAllGymsAsync();
    Task<GymEntity?> GetGymByIdAsync(int gymId);
    Task<GymEntity> CreateGymAsync(GymEntity gym);
    Task<GymEntity?> UpdateGymAsync(GymEntity gym);
    Task<bool> DeleteGymAsync(int gymId);

    Task<IEnumerable<GymSlot>> GetSlotsByGymIdAsync(
        int gymId,
        DateTime? slotDate = null);

    Task<GymSlot?> GetSlotByIdAsync(int slotId);
    Task<GymSlot> CreateSlotAsync(GymSlot slot);
    Task<GymSlot?> UpdateSlotAsync(GymSlot slot);

    Task<IEnumerable<GymBooking>> GetAllBookingsAsync();
    Task<IEnumerable<GymBooking>> GetBookingsBySlotIdAsync(int slotId);
    Task<IEnumerable<GymBooking>> GetBookingsByUserIdAsync(int userId);
    Task<GymBooking?> GetBookingByIdAsync(int bookingId);
    Task<GymBooking?> GetBookingForUserAsync(int bookingId, int userId);
    Task<GymBooking?> GetExistingActiveBookingAsync(int slotId, int userId);
    Task<GymBooking> CreateBookingAsync(GymBooking booking);
    Task<GymBooking> UpdateBookingAsync(GymBooking booking);
}
