namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Entities;

public class Department
{
    public int DepartmentId { get; set; }

    public int FacultyId { get; set; }

    public string DepartmentCode { get; set; } = string.Empty;

    public string DepartmentName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public Faculty Faculty { get; set; } = null!;
}
