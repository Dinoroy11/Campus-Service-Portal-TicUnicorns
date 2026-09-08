using GymEntity = CampusServicePortal.Modules.Gym.Entities.Gym;
using CampusServicePortal.Modules.Gym.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Gym.Repositories
{
    public class GymRepository : IGymRepository
    {
        private readonly CampusDbContext _context;

        public GymRepository(CampusDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GymEntity>> GetAllGymsAsync()
        {
            return await _context.Gyms
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<GymEntity?> GetGymByIdAsync(int gymId)
        {
            return await _context.Gyms
                .FirstOrDefaultAsync(g => g.GymId == gymId);
        }

        public async Task<GymEntity> CreateGymAsync(GymEntity gym)
        {
            await _context.Gyms.AddAsync(gym);
            await _context.SaveChangesAsync();

            return gym;
        }

        public async Task<GymEntity?> UpdateGymAsync(GymEntity gym)
        {
            var existingGym = await _context.Gyms
                .FirstOrDefaultAsync(g => g.GymId == gym.GymId);

            if (existingGym == null)
            {
                return null;
            }

            existingGym.Name = gym.Name;
            existingGym.Location = gym.Location;
            existingGym.Capacity = gym.Capacity;
            existingGym.Description = gym.Description;
            existingGym.IsActive = gym.IsActive;

            await _context.SaveChangesAsync();

            return existingGym;
        }

        public async Task<bool> DeleteGymAsync(int gymId)
        {
            var gym = await _context.Gyms
                .FirstOrDefaultAsync(g => g.GymId == gymId);

            if (gym == null)
            {
                return false;
            }

            _context.Gyms.Remove(gym);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}