using CampusServicePortal.Modules.Identity.Entities;

namespace CampusServicePortal.Modules.Identity.Interfaces.Repository;

public interface ICanteenStaffAssignmentRepository
{
    Task<bool> ExistsAsync(int userId, int canteenId);

    Task AddAsync(CanteenStaffAssignment assignment);
}
