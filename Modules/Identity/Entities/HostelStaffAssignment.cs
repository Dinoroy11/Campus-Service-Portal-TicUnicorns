namespace CampusServicePortal.Modules.Identity.Entities;

public class HostelStaffAssignment
{
    public int HostelStaffAssignmentId { get; set; }

    public int UserId { get; set; }

    public int HostelId { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}
