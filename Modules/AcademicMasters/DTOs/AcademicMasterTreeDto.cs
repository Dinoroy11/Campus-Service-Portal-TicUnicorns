namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.DTOs;

public class AcademicMasterTreeDto
{
    public int UniversityId { get; set; }
    public string UniversityCode { get; set; } = string.Empty;
    public string UniversityName { get; set; } = string.Empty;
    public List<FacultyWithDepartmentsDto> Faculties { get; set; } = new();
}

public class FacultyWithDepartmentsDto
{
    public int FacultyId { get; set; }
    public string FacultyCode { get; set; } = string.Empty;
    public string FacultyName { get; set; } = string.Empty;
    public List<DepartmentDto> Departments { get; set; } = new();
}
