using CampusServicePortal_TicUnicorns.Modules.Students.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int studentId);
    Task<Student?> GetByUserIdAsync(int userId);
    Task<IEnumerable<Student>> GetAllAsync();
    Task AddAsync(Student student);
    Task UpdateAsync(Student student);
    Task<bool> ExistsAsync(int studentId);
    Task<bool> ExistsByMasterStudentIdAsync(int masterStudentId);
}
