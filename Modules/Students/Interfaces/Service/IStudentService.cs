using CampusServicePortal_TicUnicorns.Modules.Students.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;

public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetAllAsync();

    Task<StudentDto?> GetByIdAsync(int studentId);

    Task<StudentDto?> GetByUserIdAsync(int userId);

    Task<StudentDto> CreateAsync(CreateStudentDto dto);

    Task<StudentDto?> UpdateAsync(
        int studentId,
        UpdateStudentDto dto);

    Task<StudentDto?> UpdateHostelEligibilityAsync(
        int userId,
        string gender);

    Task<int?> GetDepartmentIdAsync(int studentId);
}
