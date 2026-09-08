using CampusServicePortal.Modules.Fees.Entities;

namespace CampusServicePortal.Modules.Fees.Interfaces.Repository;

public interface IStudentFeeRepository
{
    Task<List<StudentFee>> GetAllAsync();

    Task<StudentFee?> GetByIdAsync(int studentFeeId);

    Task<List<StudentFee>> GetByStudentIdAsync(int studentId);

    Task<StudentFee> CreateAsync(StudentFee studentFee);

    Task UpdateAsync(StudentFee studentFee);

    Task<bool> ExistsAsync(int studentFeeId);
}