using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.Laundry.Entities;
using CampusServicePortal_TicUnicorns.Modules.Laundry.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Laundry.Repositories
{
    public class LaundryRepository : ILaundryRepository
    {
        private readonly CampusDbContext _context;

        public LaundryRepository(CampusDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LaundryEntities>> GetAllAsync()
        {
            return await _context.LaundryEntities
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LaundryEntities?> GetByIdAsync(int laundryId)
        {
            return await _context.LaundryEntities
                .FirstOrDefaultAsync(l => l.LaundryId == laundryId);
        }

        public async Task<IEnumerable<LaundryEntities>> GetByStudentIdAsync(
            int studentId)
        {
            return await _context.LaundryEntities
                .AsNoTracking()
                .Where(l => l.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<LaundryEntities> CreateAsync(
            LaundryEntities laundry)
        {
            await _context.LaundryEntities.AddAsync(laundry);
            await _context.SaveChangesAsync();

            return laundry;
        }

        public async Task<bool> UpdateAsync(
            LaundryEntities laundry)
        {
            var existingLaundry = await _context.LaundryEntities
                .FirstOrDefaultAsync(l => l.LaundryId == laundry.LaundryId);

            if (existingLaundry == null)
                return false;

            existingLaundry.ServiceType = laundry.ServiceType;
            existingLaundry.Quantity = laundry.Quantity;
            existingLaundry.PickupMethod = laundry.PickupMethod;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateStatusAsync(
            int laundryId,
            string status)
        {
            var laundry = await _context.LaundryEntities
                .FirstOrDefaultAsync(l => l.LaundryId == laundryId);

            if (laundry == null)
                return false;

            laundry.Status = status;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AssignAsync(
            int laundryId,
            int assignedTo)
        {
            var laundry = await _context.LaundryEntities
                .FirstOrDefaultAsync(l => l.LaundryId == laundryId);

            if (laundry == null)
                return false;

            laundry.AssignedTo = assignedTo;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int laundryId)
        {
            var laundry = await _context.LaundryEntities
                .FirstOrDefaultAsync(l => l.LaundryId == laundryId);

            if (laundry == null)
                return false;

            _context.LaundryEntities.Remove(laundry);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}