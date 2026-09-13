using CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Interfaces.Repository;

public interface IAcademicMasterRepository
{
    Task<IReadOnlyList<University>> GetActiveUniversitiesAsync();
    Task<University?> GetActiveUniversityByIdAsync(int universityId);

    Task<IReadOnlyList<Faculty>> GetActiveFacultiesByUniversityAsync(int universityId);
    Task<Faculty?> GetActiveFacultyByIdAsync(int facultyId);

    Task<IReadOnlyList<Department>> GetActiveDepartmentsAsync();
    Task<IReadOnlyList<Department>> GetActiveDepartmentsByFacultyAsync(int facultyId);
    Task<Department?> GetActiveDepartmentByIdAsync(int departmentId);

    Task<IReadOnlyList<University>> GetActiveTreeAsync();
}
