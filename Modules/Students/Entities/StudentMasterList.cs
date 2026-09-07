namespace CampusServicePortal_TicUnicorns.Modules.Students.Entities;

public class StudentMasterList
{
    public int MasterStudentId { get; set; }

    public int UniversityId { get; set; }

    public int FacultyId { get; set; }

    public int DepartmentId { get; set; }

    public string UniversityStudentId { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}