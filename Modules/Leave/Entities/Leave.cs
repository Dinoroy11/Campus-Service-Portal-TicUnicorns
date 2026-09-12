using CampusServicePortal_TicUnicorns.Modules.Students.Entities;

namespace CampusServicePortal.Modules.Leave.Entities;

public class LeaveRequest
{
    public int LeaveRequestId { get; set; }

    public int LeaveTypeId { get; set; }

    public int StudentId { get; set; }

    // Department is derived from StudentMasterList at request creation time.
    // It is stored on the request so department-scoped staff queries remain stable.
    public int DepartmentId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public LeaveType LeaveType { get; set; } = null!;

    public Student Student { get; set; } = null!;

    public ICollection<LeaveApproval> Approvals { get; set; }
        = new List<LeaveApproval>();
}
