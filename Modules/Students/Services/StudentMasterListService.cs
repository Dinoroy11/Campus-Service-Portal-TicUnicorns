using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Services;

public class StudentMasterListService : IStudentMasterListService
{
    private readonly IStudentMasterListRepository _studentMasterListRepository;
    private readonly CampusDbContext _context;

    public StudentMasterListService(
        IStudentMasterListRepository studentMasterListRepository,
        CampusDbContext context)
    {
        _studentMasterListRepository = studentMasterListRepository;
        _context = context;
    }

    public async Task<IEnumerable<StudentMasterListDto>> GetAllAsync()
    {
        var masterStudents = await _studentMasterListRepository.GetAllAsync();
        return masterStudents.Select(MapToDto);
    }

    public async Task<StudentMasterListDto?> GetByIdAsync(int masterStudentId)
    {
        if (masterStudentId <= 0)
        {
            throw new ArgumentException("Master student ID must be greater than zero.");
        }

        var masterStudent = await _studentMasterListRepository
            .GetByIdAsync(masterStudentId);

        return masterStudent is null ? null : MapToDto(masterStudent);
    }

    public async Task<StudentMasterListDto?> GetByUniversityStudentIdAsync(
        string universityStudentId)
    {
        if (string.IsNullOrWhiteSpace(universityStudentId))
        {
            throw new ArgumentException("University student ID is required.");
        }

        var masterStudent = await _studentMasterListRepository
            .GetByUniversityStudentIdAsync(universityStudentId.Trim());

        return masterStudent is null ? null : MapToDto(masterStudent);
    }

    public async Task<StudentMasterListDto> CreateAsync(
        CreateStudentMasterListDto dto)
    {
        ValidateCreateRequest(dto);

        await ValidateAcademicHierarchyAsync(
            dto.UniversityId,
            dto.FacultyId,
            dto.DepartmentId);

        var universityStudentId = dto.UniversityStudentId.Trim();

        var exists = await _studentMasterListRepository
            .ExistsByUniversityStudentIdAsync(universityStudentId);

        if (exists)
        {
            throw new InvalidOperationException(
                "A master student with this university student ID already exists.");
        }

        var masterStudent = new StudentMasterList
        {
            UniversityId = dto.UniversityId,
            FacultyId = dto.FacultyId,
            DepartmentId = dto.DepartmentId,
            UniversityStudentId = universityStudentId,
            StudentName = dto.StudentName.Trim(),
            MobileNumber = dto.MobileNumber.Trim(),
            IsActive = true
        };

        await _studentMasterListRepository.AddAsync(masterStudent);

        return MapToDto(masterStudent);
    }

    public async Task<StudentMasterListDto?> UpdateAsync(
        int masterStudentId,
        UpdateStudentMasterListDto dto)
    {
        if (masterStudentId <= 0)
        {
            throw new ArgumentException("Master student ID must be greater than zero.");
        }

        ValidateUpdateRequest(dto);

        await ValidateAcademicHierarchyAsync(
            dto.UniversityId,
            dto.FacultyId,
            dto.DepartmentId);

        var masterStudent = await _studentMasterListRepository
            .GetByIdAsync(masterStudentId);

        if (masterStudent is null)
        {
            return null;
        }

        var universityStudentId = dto.UniversityStudentId.Trim();

        var existingMasterStudent = await _studentMasterListRepository
            .GetByUniversityStudentIdAsync(universityStudentId);

        if (existingMasterStudent is not null &&
            existingMasterStudent.MasterStudentId != masterStudentId)
        {
            throw new InvalidOperationException(
                "A master student with this university student ID already exists.");
        }

        masterStudent.UniversityId = dto.UniversityId;
        masterStudent.FacultyId = dto.FacultyId;
        masterStudent.DepartmentId = dto.DepartmentId;
        masterStudent.UniversityStudentId = universityStudentId;
        masterStudent.StudentName = dto.StudentName.Trim();
        masterStudent.MobileNumber = dto.MobileNumber.Trim();
        masterStudent.IsActive = dto.IsActive;

        await _studentMasterListRepository.UpdateAsync(masterStudent);

        return MapToDto(masterStudent);
    }

    private async Task ValidateAcademicHierarchyAsync(
        int universityId,
        int facultyId,
        int departmentId)
    {
        var universityExists = await _context.Set<University>()
            .AsNoTracking()
            .AnyAsync(x =>
                x.UniversityId == universityId &&
                x.IsActive);

        if (!universityExists)
        {
            throw new InvalidOperationException(
                "The selected university does not exist or is inactive.");
        }

        var facultyExists = await _context.Set<Faculty>()
            .AsNoTracking()
            .AnyAsync(x =>
                x.FacultyId == facultyId &&
                x.UniversityId == universityId &&
                x.IsActive);

        if (!facultyExists)
        {
            throw new InvalidOperationException(
                "The selected faculty does not belong to the selected university or is inactive.");
        }

        var departmentExists = await _context.Set<Department>()
            .AsNoTracking()
            .AnyAsync(x =>
                x.DepartmentId == departmentId &&
                x.FacultyId == facultyId &&
                x.IsActive);

        if (!departmentExists)
        {
            throw new InvalidOperationException(
                "The selected department does not belong to the selected faculty or is inactive.");
        }
    }

    private static void ValidateCreateRequest(CreateStudentMasterListDto dto)
    {
        if (dto.UniversityId <= 0)
        {
            throw new ArgumentException("University ID must be greater than zero.");
        }

        if (dto.FacultyId <= 0)
        {
            throw new ArgumentException("Faculty ID must be greater than zero.");
        }

        if (dto.DepartmentId <= 0)
        {
            throw new ArgumentException("Department ID must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(dto.UniversityStudentId))
        {
            throw new ArgumentException("University student ID is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.StudentName))
        {
            throw new ArgumentException("Student name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.MobileNumber))
        {
            throw new ArgumentException("Mobile number is required.");
        }
    }

    private static void ValidateUpdateRequest(UpdateStudentMasterListDto dto)
    {
        if (dto.UniversityId <= 0)
        {
            throw new ArgumentException("University ID must be greater than zero.");
        }

        if (dto.FacultyId <= 0)
        {
            throw new ArgumentException("Faculty ID must be greater than zero.");
        }

        if (dto.DepartmentId <= 0)
        {
            throw new ArgumentException("Department ID must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(dto.UniversityStudentId))
        {
            throw new ArgumentException("University student ID is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.StudentName))
        {
            throw new ArgumentException("Student name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.MobileNumber))
        {
            throw new ArgumentException("Mobile number is required.");
        }
    }

    private static StudentMasterListDto MapToDto(StudentMasterList masterStudent)
    {
        return new StudentMasterListDto
        {
            MasterStudentId = masterStudent.MasterStudentId,
            UniversityId = masterStudent.UniversityId,
            FacultyId = masterStudent.FacultyId,
            DepartmentId = masterStudent.DepartmentId,
            UniversityStudentId = masterStudent.UniversityStudentId,
            StudentName = masterStudent.StudentName,
            MobileNumber = masterStudent.MobileNumber,
            IsActive = masterStudent.IsActive
        };
    }
}
