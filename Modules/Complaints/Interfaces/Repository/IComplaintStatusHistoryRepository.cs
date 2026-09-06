using CampusServicePortal.Modules.Complaints.Entities;

namespace CampusServicePortal.Modules.Complaints.Interfaces.Repository;

public interface IComplaintStatusHistoryRepository
{
    Task<List<ComplaintStatusHistory>> GetByComplaintIdAsync(
        int complaintId);

    Task<ComplaintStatusHistory> CreateAsync(
        ComplaintStatusHistory history);
}