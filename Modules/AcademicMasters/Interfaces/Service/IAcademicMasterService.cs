using CampusServicePortal_TicUnicorns.Modules.AcademicMasters.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Interfaces.Service;

public interface IAcademicMasterService
{
    Task<IReadOnlyList<UniversityDto>> GetUniversitiesAsync();
    Task<IReadOnlyList<FacultyDto>> GetFacultiesByUniversityAsync(int universityId);
    Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync();
    Task<IReadOnlyList<DepartmentDto>> GetDepartmentsByFacultyAsync(int facultyId);
    Task<IReadOnlyList<AcademicMasterTreeDto>> GetTreeAsync();
}
