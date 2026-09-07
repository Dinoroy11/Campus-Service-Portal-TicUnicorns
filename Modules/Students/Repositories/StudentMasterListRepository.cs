using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Repositories;

public class StudentMasterListRepository : IStudentMasterListRepository
{
    public Task<StudentMasterList?> GetByIdAsync(int masterStudentId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<StudentMasterList>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<StudentMasterList?> GetByUniversityStudentIdAsync(
        string universityStudentId)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(StudentMasterList studentMasterList)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(StudentMasterList studentMasterList)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(int masterStudentId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByUniversityStudentIdAsync(
        string universityStudentId)
    {
        throw new NotImplementedException();
    }
}