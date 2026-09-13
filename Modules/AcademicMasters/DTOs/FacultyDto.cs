namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.DTOs;

public class FacultyDto
{
    public int FacultyId { get; set; }
    public int UniversityId { get; set; }
    public string FacultyCode { get; set; } = string.Empty;
    public string FacultyName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
