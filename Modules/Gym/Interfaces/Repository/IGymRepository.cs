using GymEntity = CampusServicePortal.Modules.Gym.Entities.Gym;

namespace CampusServicePortal.Modules.Gym.Interfaces.Repository
{
    public interface IGymRepository
    {
        Task<IEnumerable<GymEntity>> GetAllGymsAsync();

        Task<GymEntity?> GetGymByIdAsync(int gymId);

        Task<GymEntity> CreateGymAsync(GymEntity gym);

        Task<GymEntity?> UpdateGymAsync(GymEntity gym);

        Task<bool> DeleteGymAsync(int gymId);
    }
}