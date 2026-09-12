namespace CampusServicePortal.Modules.Complaints.DTOs;

public class CreateComplaintDto
{
    public int CategoryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
