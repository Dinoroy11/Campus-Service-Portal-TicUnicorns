using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;


namespace CampusServicePortal_TicUnicorns.Modules.Canteens.Interfaces.Repository
{
    public interface ICanteenRepository
    {
        Task<IEnumerable<CanteenEntities>> GetAllAsync();

        Task<CanteenEntities?> GetByIdAsync(int canteenId);

        Task<IEnumerable<CanteenEntities>> GetByStudentIdAsync(int studentId);

        Task<CanteenEntities> CreateAsync(CanteenEntities canteen);

        Task<CanteenEntities?> UpdateAsync(int canteenId, CanteenEntities canteen);

        Task<CanteenEntities?> UpdateStatusAsync(
            int canteenId,
            string status);

        Task<bool> DeleteAsync(int canteenId);
    }
}