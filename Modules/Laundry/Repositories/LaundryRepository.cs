using CampusServicePortal_TicUnicorns.Modules.Laundry.Entities;
using CampusServicePortal_TicUnicorns.Modules.Laundry.Interfaces.Repository;

namespace CampusServicePortal_TicUnicorns.Modules.Laundry.Repositories
{
    public class LaundryRepository : ILaundryRepository
    {
        public Task<IEnumerable<LaundryEntities>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<LaundryEntities?> GetByIdAsync(int laundryId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LaundryEntities>> GetByStudentIdAsync(
            int studentId)
        {
            throw new NotImplementedException();
        }

        public Task<LaundryEntities> CreateAsync(
            LaundryEntities laundry)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(
            LaundryEntities laundry)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateStatusAsync(
            int laundryId,
            string status)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AssignAsync(
            int laundryId,
            int assignedTo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int laundryId)
        {
            throw new NotImplementedException();
        }
    }
}