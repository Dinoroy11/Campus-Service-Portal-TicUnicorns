namespace CampusServicePortal.Modules.Complaints.DTOs;

public class ComplaintDto
{
    public int ComplaintId { get; set; }

    public int CategoryId { get; set; }

    public int StudentId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string ActionRemarks { get; set; } = string.Empty;

    public int? StatusChangedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}