using CampusServicePortal.Modules.Fees.Entities;
using CampusServicePortal.Modules.Fees.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Fees.Repositories;

public class StudentFeeRepository : IStudentFeeRepository
{
    private readonly CampusDbContext _context;

    public StudentFeeRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudentFee>> GetAllAsync()
    {
        return await _context.Set<StudentFee>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<StudentFee?> GetByIdAsync(int studentFeeId)
    {
        return await _context.Set<StudentFee>()
            .FirstOrDefaultAsync(x => x.StudentFeeId == studentFeeId);
    }

    public async Task<List<StudentFee>> GetByStudentIdAsync(int studentId)
    {
        return await _context.Set<StudentFee>()
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<StudentFee?> GetByReferenceAsync(
        int studentId,
        string reference)
    {
        return await _context.Set<StudentFee>()
            .FirstOrDefaultAsync(x =>
                x.StudentId == studentId &&
                x.ExamReference == reference);
    }

    public async Task<StudentFee> CreateAsync(StudentFee studentFee)
    {
        await _context.Set<StudentFee>()
            .AddAsync(studentFee);

        await _context.SaveChangesAsync();

        return studentFee;
    }

    public async Task UpdateAsync(StudentFee studentFee)
    {
        _context.Set<StudentFee>()
            .Update(studentFee);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int studentFeeId)
    {
        return await _context.Set<StudentFee>()
            .AnyAsync(x => x.StudentFeeId == studentFeeId);
    }
}