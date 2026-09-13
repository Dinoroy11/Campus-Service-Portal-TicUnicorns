namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.DTOs;

public class UniversityDto
{
    public int UniversityId { get; set; }
    public string UniversityCode { get; set; } = string.Empty;
    public string UniversityName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
