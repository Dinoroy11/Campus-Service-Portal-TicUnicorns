using CampusServicePortal.Modules.Identity.Entities;

namespace CampusServicePortal.Modules.Identity.Interfaces.Repository;

public interface IHostelStaffAssignmentRepository
{
    Task<bool> ExistsAsync(int userId, int hostelId);

    Task AddAsync(HostelStaffAssignment assignment);
}