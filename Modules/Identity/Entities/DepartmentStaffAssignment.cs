namespace CampusServicePortal.Modules.Identity.Entities;

public class DepartmentStaffAssignment
{
    public int DepartmentStaffAssignmentId { get; set; }

    public int UserId { get; set; }

    public int DepartmentId { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}