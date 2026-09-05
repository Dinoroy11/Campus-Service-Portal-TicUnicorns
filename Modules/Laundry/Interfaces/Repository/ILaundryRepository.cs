using CampusServicePortal_TicUnicorns.Modules.Laundry.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Laundry.Interfaces.Repository
{
    public interface ILaundryRepository
    {
        Task<IEnumerable<LaundryEntities>> GetAllAsync();

        Task<LaundryEntities?> GetByIdAsync(int laundryId);

        Task<IEnumerable<LaundryEntities>> GetByStudentIdAsync(int studentId);

        Task<LaundryEntities> CreateAsync(
            LaundryEntities laundry);

        Task<bool> UpdateAsync(
            LaundryEntities laundry);

        Task<bool> UpdateStatusAsync(
            int laundryId,
            string status);

        Task<bool> AssignAsync(
            int laundryId,
            int assignedTo);

        Task<bool> DeleteAsync(int laundryId);
    }
}