namespace CampusServicePortal.Modules.Leave.DTOs;

// Used by Department Staff/Admin when reviewing a leave request.
public class UpdateLeaveDto
{
    // Allowed values: Approved, Rejected
    public string Status { get; set; } = string.Empty;

    public string? Remarks { get; set; }
}

public class CreateLeaveTypeDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}

public class UpdateLeaveTypeStatusDto
{
    public bool IsActive { get; set; }
}
