using CampusServicePortal_TicUnicorns.Modules.Students.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;

public interface IStudentMasterListRepository
{
    Task<StudentMasterList?> GetByIdAsync(int masterStudentId);

    Task<IEnumerable<StudentMasterList>> GetAllAsync();

    Task<StudentMasterList?> GetByUniversityStudentIdAsync(
        string universityStudentId);

    Task AddAsync(StudentMasterList studentMasterList);

    Task UpdateAsync(StudentMasterList studentMasterList);

    Task<bool> ExistsAsync(int masterStudentId);

    Task<bool> ExistsByUniversityStudentIdAsync(
        string universityStudentId);
}