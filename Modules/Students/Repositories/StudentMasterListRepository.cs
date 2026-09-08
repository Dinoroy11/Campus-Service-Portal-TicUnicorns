using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Repositories;

public class StudentMasterListRepository : IStudentMasterListRepository
{
    private readonly CampusDbContext _context;

    public StudentMasterListRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<StudentMasterList?> GetByIdAsync(int masterStudentId)
    {
        return await _context.StudentMasterLists
            .FirstOrDefaultAsync(x => x.MasterStudentId == masterStudentId);
    }

    public async Task<IEnumerable<StudentMasterList>> GetAllAsync()
    {
        return await _context.StudentMasterLists
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<StudentMasterList?> GetByUniversityStudentIdAsync(
        string universityStudentId)
    {
        return await _context.StudentMasterLists
            .FirstOrDefaultAsync(x =>
                x.UniversityStudentId == universityStudentId);
    }

    public async Task AddAsync(StudentMasterList studentMasterList)
    {
        await _context.StudentMasterLists.AddAsync(studentMasterList);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(StudentMasterList studentMasterList)
    {
        _context.StudentMasterLists.Update(studentMasterList);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int masterStudentId)
    {
        return await _context.StudentMasterLists
            .AnyAsync(x => x.MasterStudentId == masterStudentId);
    }

    public async Task<bool> ExistsByUniversityStudentIdAsync(
        string universityStudentId)
    {
        return await _context.StudentMasterLists
            .AnyAsync(x =>
                x.UniversityStudentId == universityStudentId);
    }
}