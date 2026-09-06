namespace CampusServicePortal.Modules.Complaints.DTOs;

public class ComplaintCategoryDto
{
    public int ComplaintCategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}