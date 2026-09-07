using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Repositories;

public class StudentRepository : IStudentRepository
{
    public Task<Student?> GetByIdAsync(int studentId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Student>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Student student)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Student student)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(int studentId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByMasterStudentIdAsync(int masterStudentId)
    {
        throw new NotImplementedException();
    }
}