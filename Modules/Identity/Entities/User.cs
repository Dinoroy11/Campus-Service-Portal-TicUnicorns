namespace CampusServicePortal.Modules.Identity.Entities;

public class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsPhoneVerified { get; set; }

    public string PasswordHash { get; set; } = string.Empty;

    public bool MustChangePassword { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public ICollection<DepartmentStaffAssignment> DepartmentStaffAssignments { get; set; }
        = new List<DepartmentStaffAssignment>();

    public ICollection<HostelStaffAssignment> HostelStaffAssignments { get; set; }
        = new List<HostelStaffAssignment>();

    public ICollection<CanteenStaffAssignment> CanteenStaffAssignments { get; set; }
        = new List<CanteenStaffAssignment>();
}