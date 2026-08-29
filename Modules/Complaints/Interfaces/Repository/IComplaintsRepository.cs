using CampusServicePortal_TicUnicorns.Modules.Complaints.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Complaints.Interfaces.Repository
{
    public interface IComplaintsRepository
    {
        Task<IEnumerable<ComplaintsEntities>> GetAllAsync();

        Task<ComplaintsEntities?> GetByIdAsync(int complaintId);

        Task<IEnumerable<ComplaintsEntities>> GetByStudentIdAsync(int studentId);

        Task<ComplaintsEntities> CreateAsync(ComplaintsEntities complaint);

        Task<bool> UpdateAsync(ComplaintsEntities complaint);

        Task<bool> UpdateStatusAsync(
            int complaintId,
            string status,
            string? resolution);

        Task<bool> AssignAsync(
            int complaintId,
            int assignedTo);

        Task<bool> DeleteAsync(int complaintId);
    }
}