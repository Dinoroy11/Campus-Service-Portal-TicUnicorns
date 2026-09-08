using CampusServicePortal.Modules.Identity.Entities;

namespace CampusServicePortal.Modules.Identity.Interfaces.Repository;

public interface IDepartmentStaffAssignmentRepository
{
    Task<bool> ExistsAsync(int userId, int departmentId);

    Task AddAsync(DepartmentStaffAssignment assignment);
}