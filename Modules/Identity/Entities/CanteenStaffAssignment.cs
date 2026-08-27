namespace CampusServicePortal.Modules.Identity.Entities;

public class CanteenStaffAssignment
{
    public int CanteenStaffAssignmentId { get; set; }

    public int UserId { get; set; }

    public int CanteenId { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
} 