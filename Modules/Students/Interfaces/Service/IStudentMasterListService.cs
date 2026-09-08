using CampusServicePortal_TicUnicorns.Modules.Students.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;

public interface IStudentMasterListService
{
    Task<IEnumerable<StudentMasterListDto>> GetAllAsync();

    Task<StudentMasterListDto?> GetByIdAsync(
        int masterStudentId);

    Task<StudentMasterListDto?> GetByUniversityStudentIdAsync(
        string universityStudentId);

    Task<StudentMasterListDto> CreateAsync(
        CreateStudentMasterListDto dto);

    Task<StudentMasterListDto?> UpdateAsync(
        int masterStudentId,
        UpdateStudentMasterListDto dto);
}