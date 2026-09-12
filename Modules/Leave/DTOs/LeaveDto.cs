namespace CampusServicePortal.Modules.Leave.DTOs;

public class LeaveDto
{
    public int LeaveRequestId { get; set; }

    public int LeaveTypeId { get; set; }

    public string LeaveTypeName { get; set; } = string.Empty;

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public int DepartmentId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public IReadOnlyCollection<LeaveApprovalDto> Approvals { get; set; }
        = Array.Empty<LeaveApprovalDto>();
}

public class LeaveApprovalDto
{
    public int LeaveApprovalId { get; set; }

    public int ApprovedByUserId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public DateTime ApprovedAt { get; set; }
}

public class LeaveTypeDto
{
    public int LeaveTypeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
