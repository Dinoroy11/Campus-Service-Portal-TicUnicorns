using CampusServicePortal.Modules.Complaints.Entities;

namespace CampusServicePortal.Modules.Complaints.Interfaces.Repository;

public interface IComplaintCategoryRepository
{
    Task<List<ComplaintCategory>> GetAllAsync();

    Task<ComplaintCategory?> GetByIdAsync(int complaintCategoryId);

    Task<ComplaintCategory> CreateAsync(ComplaintCategory category);

    Task UpdateAsync(ComplaintCategory category);

    Task<bool> ExistsAsync(int complaintCategoryId);
}