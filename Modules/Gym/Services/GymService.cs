using CampusServicePortal.Modules.Gym.DTOs;
using GymEntity = CampusServicePortal.Modules.Gym.Entities.Gym;
using CampusServicePortal.Modules.Gym.Interfaces.Repository;
using CampusServicePortal.Modules.Gym.Interfaces.Service;

namespace CampusServicePortal.Modules.Gym.Services
{
    public class GymService : IGymService
    {
        private readonly IGymRepository _gymRepository;

        public GymService(IGymRepository gymRepository)
        {
            _gymRepository = gymRepository;
        }

        public async Task<IEnumerable<GymDto>> GetAllGymsAsync()
        {
            var gyms = await _gymRepository.GetAllGymsAsync();

            return gyms.Select(gym => new GymDto
            {
                GymId = gym.GymId,
                Name = gym.Name,
                Location = gym.Location,
                Capacity = gym.Capacity,
                Description = gym.Description,
                IsActive = gym.IsActive
            });
        }

        public async Task<GymDto?> GetGymByIdAsync(int gymId)
        {
            var gym = await _gymRepository.GetGymByIdAsync(gymId);

            if (gym == null)
            {
                return null;
            }

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

        public async Task<GymDto> CreateGymAsync(CreateGymDto gymDto)
        {
            var gym = new GymEntity
            {
                Name = gymDto.Name,
                Location = gymDto.Location,
                Capacity = gymDto.Capacity,
                Description = gymDto.Description,
                IsActive = true
            };

            var createdGym = await _gymRepository.CreateGymAsync(gym);

            return new GymDto
            {
                GymId = createdGym.GymId,
                Name = createdGym.Name,
                Location = createdGym.Location,
                Capacity = createdGym.Capacity,
                Description = createdGym.Description,
                IsActive = createdGym.IsActive
            };
        }

        public async Task<GymDto?> UpdateGymAsync(
            int gymId,
            CreateGymDto gymDto)
        {
            var existingGym = await _gymRepository.GetGymByIdAsync(gymId);

            if (existingGym == null)
            {
                return null;
            }

            existingGym.Name = gymDto.Name;
            existingGym.Location = gymDto.Location;
            existingGym.Capacity = gymDto.Capacity;
            existingGym.Description = gymDto.Description;

            var updatedGym =
                await _gymRepository.UpdateGymAsync(existingGym);

            if (updatedGym == null)
            {
                return null;
            }

            return new GymDto
            {
                GymId = updatedGym.GymId,
                Name = updatedGym.Name,
                Location = updatedGym.Location,
                Capacity = updatedGym.Capacity,
                Description = updatedGym.Description,
                IsActive = updatedGym.IsActive
            };
        }

        public async Task<bool> DeleteGymAsync(int gymId)
        {
            var existingGym =
                await _gymRepository.GetGymByIdAsync(gymId);

            if (existingGym == null)
            {
                return false;
            }

            return await _gymRepository.DeleteGymAsync(gymId);
        }
    }
}