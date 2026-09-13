namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.DTOs;

public class DepartmentDto
{
    public int DepartmentId { get; set; }
    public int FacultyId { get; set; }
    public string DepartmentCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
