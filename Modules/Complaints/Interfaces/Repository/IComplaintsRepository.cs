using CampusServicePortal.Modules.Complaints.Entities;

namespace CampusServicePortal.Modules.Complaints.Interfaces.Repository;

public interface IComplaintRepository
{
    Task<List<Complaint>> GetAllAsync();

    Task<Complaint?> GetByIdAsync(int complaintId);

    Task<List<Complaint>> GetByStudentIdAsync(int studentId);

    Task<Complaint> CreateAsync(Complaint complaint);

    Task UpdateAsync(Complaint complaint);

    Task<bool> ExistsAsync(int complaintId);
}