using CampusServicePortal.Modules.Identity.Entities;

namespace CampusServicePortal.Modules.Leave.Entities;

public class LeaveApproval
{
    public int LeaveApprovalId { get; set; }

    public int LeaveRequestId { get; set; }

    public int ApprovedByUserId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public DateTime ApprovedAt { get; set; } = DateTime.UtcNow;

    public LeaveRequest LeaveRequest { get; set; } = null!;

    public User ApprovedByUser { get; set; } = null!;
}
