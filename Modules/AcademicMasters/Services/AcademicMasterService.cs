using CampusServicePortal_TicUnicorns.Modules.AcademicMasters.DTOs;
using CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Entities;
using CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Interfaces.Service;

namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Services;

public class AcademicMasterService : IAcademicMasterService
{
    private readonly IAcademicMasterRepository _repository;

    public AcademicMasterService(IAcademicMasterRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<UniversityDto>> GetUniversitiesAsync()
    {
        var items = await _repository.GetActiveUniversitiesAsync();
        return items.Select(MapUniversity).ToList();
    }

    public async Task<IReadOnlyList<FacultyDto>> GetFacultiesByUniversityAsync(
        int universityId)
    {
        if (universityId <= 0)
        {
            throw new ArgumentException("University ID must be greater than zero.");
        }

        var university = await _repository.GetActiveUniversityByIdAsync(universityId);
        if (university is null)
        {
            throw new KeyNotFoundException("Active university not found.");
        }

        var items = await _repository.GetActiveFacultiesByUniversityAsync(universityId);
        return items.Select(MapFaculty).ToList();
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync()
    {
        var items = await _repository.GetActiveDepartmentsAsync();
        return items.Select(MapDepartment).ToList();
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetDepartmentsByFacultyAsync(
        int facultyId)
    {
        if (facultyId <= 0)
        {
            throw new ArgumentException("Faculty ID must be greater than zero.");
        }

        var faculty = await _repository.GetActiveFacultyByIdAsync(facultyId);
        if (faculty is null)
        {
            throw new KeyNotFoundException("Active faculty not found.");
        }

        var items = await _repository.GetActiveDepartmentsByFacultyAsync(facultyId);
        return items.Select(MapDepartment).ToList();
    }

    public async Task<IReadOnlyList<AcademicMasterTreeDto>> GetTreeAsync()
    {
        var universities = await _repository.GetActiveTreeAsync();

        return universities.Select(u => new AcademicMasterTreeDto
        {
            UniversityId = u.UniversityId,
            UniversityCode = u.UniversityCode,
            UniversityName = u.UniversityName,
            Faculties = u.Faculties
                .Where(f => f.IsActive)
                .OrderBy(f => f.FacultyName)
                .Select(f => new FacultyWithDepartmentsDto
                {
                    FacultyId = f.FacultyId,
                    FacultyCode = f.FacultyCode,
                    FacultyName = f.FacultyName,
                    Departments = f.Departments
                        .Where(d => d.IsActive)
                        .OrderBy(d => d.DepartmentName)
                        .Select(MapDepartment)
                        .ToList()
                })
                .ToList()
        }).ToList();
    }

    private static UniversityDto MapUniversity(University item)
    {
        return new UniversityDto
        {
            UniversityId = item.UniversityId,
            UniversityCode = item.UniversityCode,
            UniversityName = item.UniversityName,
            IsActive = item.IsActive
        };
    }

    private static FacultyDto MapFaculty(Faculty item)
    {
        return new FacultyDto
        {
            FacultyId = item.FacultyId,
            UniversityId = item.UniversityId,
            FacultyCode = item.FacultyCode,
            FacultyName = item.FacultyName,
            IsActive = item.IsActive
        };
    }

    private static DepartmentDto MapDepartment(Department item)
    {
        return new DepartmentDto
        {
            DepartmentId = item.DepartmentId,
            FacultyId = item.FacultyId,
            DepartmentCode = item.DepartmentCode,
            DepartmentName = item.DepartmentName,
            IsActive = item.IsActive
        };
    }
}
