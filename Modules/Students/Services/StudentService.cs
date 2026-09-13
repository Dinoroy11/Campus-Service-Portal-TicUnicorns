using CampusServicePortal_TicUnicorns.Modules.Students.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;

    private readonly IStudentMasterListRepository
        _studentMasterListRepository;

    public StudentService(
        IStudentRepository studentRepository,
        IStudentMasterListRepository studentMasterListRepository)
    {
        _studentRepository = studentRepository;
        _studentMasterListRepository = studentMasterListRepository;
    }

    public async Task<IEnumerable<StudentDto>> GetAllAsync()
    {
        var students = await _studentRepository.GetAllAsync();

        return students.Select(MapToDto);
    }

    public async Task<StudentDto?> GetByIdAsync(int studentId)
    {
        if (studentId <= 0)
        {
            throw new ArgumentException(
                "Student ID must be greater than zero.");
        }

        var student =
            await _studentRepository.GetByIdAsync(studentId);

        return student is null
            ? null
            : MapToDto(student);
    }

    public async Task<StudentDto?> GetByUserIdAsync(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException(
                "User ID must be greater than zero.");
        }

        var student =
            await _studentRepository.GetByUserIdAsync(userId);

        return student is null
            ? null
            : MapToDto(student);
    }

    public async Task<StudentDto> CreateAsync(
        CreateStudentDto dto)
    {
        ValidateCreateRequest(dto);

        var masterStudent =
            await _studentMasterListRepository
                .GetByIdAsync(dto.MasterStudentId);

        if (masterStudent is null)
        {
            throw new InvalidOperationException(
                "Master student does not exist.");
        }

        if (!masterStudent.IsActive)
        {
            throw new InvalidOperationException(
                "Master student is inactive.");
        }

        var exists =
            await _studentRepository
                .ExistsByMasterStudentIdAsync(
                    dto.MasterStudentId);

        if (exists)
        {
            throw new InvalidOperationException(
                "A student already exists for this master student.");
        }

        var student = new Student
        {
            MasterStudentId = dto.MasterStudentId,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender?.Trim(),
            Email = dto.Email?.Trim(),
            PhoneNumber = dto.PhoneNumber?.Trim(),
            AdmissionDate = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _studentRepository.AddAsync(student);

        return MapToDto(student);
    }

    public async Task<StudentDto?> UpdateAsync(
        int studentId,
        UpdateStudentDto dto)
    {
        if (studentId <= 0)
        {
            throw new ArgumentException(
                "Student ID must be greater than zero.");
        }

        ValidateUpdateRequest(dto);

        var student =
            await _studentRepository.GetByIdAsync(studentId);

        if (student is null)
        {
            return null;
        }

        student.FirstName = dto.FirstName.Trim();
        student.LastName = dto.LastName.Trim();
        student.DateOfBirth = dto.DateOfBirth;
        student.Gender = dto.Gender?.Trim();
        student.Email = dto.Email?.Trim();
        student.PhoneNumber = dto.PhoneNumber?.Trim();
        student.IsActive = dto.IsActive;

        await _studentRepository.UpdateAsync(student);

        return MapToDto(student);
    }

    public async Task<int?> GetDepartmentIdAsync(int studentId)
    {
        if (studentId <= 0)
            throw new ArgumentException("Student ID must be greater than zero.");

        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student is null)
            return null;

        var masterStudent = await _studentMasterListRepository
            .GetByIdAsync(student.MasterStudentId);

        if (masterStudent is null)
            return null;

        return masterStudent.DepartmentId;
    }

    private static void ValidateCreateRequest(
        CreateStudentDto dto)
    {
        if (dto.MasterStudentId <= 0)
        {
            throw new ArgumentException(
                "Master student ID must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(dto.FirstName))
        {
            throw new ArgumentException(
                "First name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.LastName))
        {
            throw new ArgumentException(
                "Last name is required.");
        }
    }

    private static void ValidateUpdateRequest(
        UpdateStudentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName))
        {
            throw new ArgumentException(
                "First name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.LastName))
        {
            throw new ArgumentException(
                "Last name is required.");
        }
    }

    private static StudentDto MapToDto(
        Student student)
    {
        return new StudentDto
        {
            StudentId = student.StudentId,
            MasterStudentId = student.MasterStudentId,
            UserId = student.UserId,
            FirstName = student.FirstName,
            LastName = student.LastName,
            DateOfBirth = student.DateOfBirth,
            Gender = student.Gender,
            Email = student.Email,
            PhoneNumber = student.PhoneNumber,
            AdmissionDate = student.AdmissionDate,
            IsActive = student.IsActive,
            CreatedAt = student.CreatedAt
        };
    }
}