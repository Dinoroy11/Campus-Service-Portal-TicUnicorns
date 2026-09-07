using CampusServicePortal_TicUnicorns.Modules.Students.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;

public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetAllAsync();

    Task<StudentDto?> GetByIdAsync(int studentId);

    Task<StudentDto> CreateAsync(CreateStudentDto dto);

    Task<StudentDto?> UpdateAsync(
        int studentId,
        UpdateStudentDto dto);

    Task<int?> GetDepartmentIdAsync(int studentId);
}
