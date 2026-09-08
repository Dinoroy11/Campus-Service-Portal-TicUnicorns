using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly CampusDbContext _context;

    public StudentRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(int studentId)
    {
        return await _context.Students
            .FirstOrDefaultAsync(x => x.StudentId == studentId);
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _context.Students
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Student student)
    {
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int studentId)
    {
        return await _context.Students
            .AnyAsync(x => x.StudentId == studentId);
    }

    public async Task<bool> ExistsByMasterStudentIdAsync(int masterStudentId)
    {
        return await _context.Students
            .AnyAsync(x => x.MasterStudentId == masterStudentId);
    }
}