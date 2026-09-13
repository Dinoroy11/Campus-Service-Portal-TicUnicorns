namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Entities;

public class Faculty
{
    public int FacultyId { get; set; }

    public int UniversityId { get; set; }

    public string FacultyCode { get; set; } = string.Empty;

    public string FacultyName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public University University { get; set; } = null!;

    public ICollection<Department> Departments { get; set; } = new List<Department>();
}
