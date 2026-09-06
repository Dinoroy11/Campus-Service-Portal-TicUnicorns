namespace CampusServicePortal.Modules.Complaints.DTOs;

public class ComplaintStatusHistoryDto
{
    public int ComplaintStatusHistoryId { get; set; }

    public int ComplaintId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;

    public int ChangedByUserId { get; set; }

    public DateTime ChangedAt { get; set; }
}