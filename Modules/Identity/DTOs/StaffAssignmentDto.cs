namespace CampusServicePortal.Modules.Identity.DTOs;

public class StaffAssignmentDto
{
    public int UserId { get; set; }

    public int? DepartmentId { get; set; }

    public int? HostelId { get; set; }

    public int? CanteenId { get; set; }
}