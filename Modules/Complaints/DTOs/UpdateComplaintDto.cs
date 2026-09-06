namespace CampusServicePortal.Modules.Complaints.DTOs;

public class UpdateComplaintDto
{
    public string Status { get; set; } = string.Empty;

    public string ActionRemarks { get; set; } = string.Empty;

    public int? StatusChangedBy { get; set; }
}