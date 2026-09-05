using CampusServicePortal.Modules.Gym.DTOs;

namespace CampusServicePortal.Modules.Gym.Interfaces.Service
{
    public interface IGymService
    {
        Task<IEnumerable<GymDto>> GetAllGymsAsync();

        Task<GymDto?> GetGymByIdAsync(int gymId);

        Task<GymDto> CreateGymAsync(CreateGymDto gymDto);

        Task<GymDto?> UpdateGymAsync(int gymId, CreateGymDto gymDto);

        Task<bool> DeleteGymAsync(int gymId);
    }
}