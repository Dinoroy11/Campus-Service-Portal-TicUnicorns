using CampusServicePortal_TicUnicorns.Modules.Complaints.Entities;
using CampusServicePortal_TicUnicorns.Modules.Complaints.Interfaces.Repository;

namespace CampusServicePortal_TicUnicorns.Modules.Complaints.Repositories
{
    public class ComplaintsRepository : IComplaintsRepository
    {
        public Task<IEnumerable<ComplaintsEntities>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ComplaintsEntities?> GetByIdAsync(int complaintId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ComplaintsEntities>> GetByStudentIdAsync(int studentId)
        {
            throw new NotImplementedException();
        }

        public Task<ComplaintsEntities> CreateAsync(ComplaintsEntities complaint)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(ComplaintsEntities complaint)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateStatusAsync(
            int complaintId,
            string status,
            string? resolution)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AssignAsync(
            int complaintId,
            int assignedTo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int complaintId)
        {
            throw new NotImplementedException();
        }
    }
}