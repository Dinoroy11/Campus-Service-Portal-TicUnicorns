using GymEntity = CampusServicePortal.Modules.Gym.Entities.Gym;
using CampusServicePortal.Modules.Gym.Interfaces.Repository;

namespace CampusServicePortal.Modules.Gym.Repositories
{
    public class GymRepository : IGymRepository
    {
        private readonly List<GymEntity> _gyms = new();

        public Task<IEnumerable<GymEntity>> GetAllGymsAsync()
        {
            return Task.FromResult<IEnumerable<GymEntity>>(_gyms);
        }

        public Task<GymEntity?> GetGymByIdAsync(int gymId)
        {
            var gym = _gyms.FirstOrDefault(x => x.GymId == gymId);

            return Task.FromResult(gym);
        }

        public Task<GymEntity> CreateGymAsync(GymEntity gym)
        {
            gym.GymId = _gyms.Count + 1;

            _gyms.Add(gym);

            return Task.FromResult(gym);
        }

        public Task<GymEntity?> UpdateGymAsync(GymEntity gym)
        {
            var existingGym = _gyms.FirstOrDefault(x => x.GymId == gym.GymId);

            if (existingGym == null)
            {
                return Task.FromResult<GymEntity?>(null);
            }

            existingGym.Name = gym.Name;
            existingGym.Location = gym.Location;
            existingGym.Capacity = gym.Capacity;
            existingGym.Description = gym.Description;
            existingGym.IsActive = gym.IsActive;

            return Task.FromResult<GymEntity?>(existingGym);
        }

        public Task<bool> DeleteGymAsync(int gymId)
        {
            var gym = _gyms.FirstOrDefault(x => x.GymId == gymId);

            if (gym == null)
            {
                return Task.FromResult(false);
            }

            _gyms.Remove(gym);

            return Task.FromResult(true);
        }
    }
}