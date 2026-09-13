namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Entities;

public class University
{
    public int UniversityId { get; set; }

    public string UniversityCode { get; set; } = string.Empty;

    public string UniversityName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<Faculty> Faculties { get; set; } = new List<Faculty>();
}
