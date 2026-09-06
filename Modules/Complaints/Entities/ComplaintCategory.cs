namespace CampusServicePortal.Modules.Complaints.Entities;

public class ComplaintCategory
{
    public int ComplaintCategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<Complaint> Complaints { get; set; }
        = new List<Complaint>();
}