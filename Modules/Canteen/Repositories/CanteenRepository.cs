using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using CampusServicePortal_TicUnicorns.Modules.Canteens.Interfaces.Repository;

namespace CampusServicePortal_TicUnicorns.Modules.Canteens.Repositories
{
    public class CanteenRepository : ICanteenRepository
    {
        public Task<IEnumerable<CanteenEntities>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CanteenEntities?> GetByIdAsync(int canteenId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CanteenEntities>> GetByStudentIdAsync(int studentId)
        {
            throw new NotImplementedException();
        }

        public Task<CanteenEntities> CreateAsync(CanteenEntities canteen)
        {
            throw new NotImplementedException();
        }

        public Task<CanteenEntities?> UpdateAsync(
            int canteenId,
            CanteenEntities canteen)
        {
            throw new NotImplementedException();
        }

        public Task<CanteenEntities?> UpdateStatusAsync(
            int canteenId,
            string status)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int canteenId)
        {
            throw new NotImplementedException();
        }
    }
}