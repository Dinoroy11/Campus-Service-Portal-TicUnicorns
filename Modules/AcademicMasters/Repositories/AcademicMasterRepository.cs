using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Entities;
using CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Repositories;

public class AcademicMasterRepository : IAcademicMasterRepository
{
    private readonly CampusDbContext _context;

    public AcademicMasterRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<University>> GetActiveUniversitiesAsync()
    {
        return await _context.Set<University>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.UniversityName)
            .ToListAsync();
    }

    public async Task<University?> GetActiveUniversityByIdAsync(int universityId)
    {
        return await _context.Set<University>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.UniversityId == universityId &&
                x.IsActive);
    }

    public async Task<IReadOnlyList<Faculty>> GetActiveFacultiesByUniversityAsync(
        int universityId)
    {
        return await _context.Set<Faculty>()
            .AsNoTracking()
            .Where(x =>
                x.UniversityId == universityId &&
                x.IsActive)
            .OrderBy(x => x.FacultyName)
            .ToListAsync();
    }

    public async Task<Faculty?> GetActiveFacultyByIdAsync(int facultyId)
    {
        return await _context.Set<Faculty>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.FacultyId == facultyId &&
                x.IsActive);
    }

    public async Task<IReadOnlyList<Department>> GetActiveDepartmentsAsync()
    {
        return await _context.Set<Department>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DepartmentName)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Department>> GetActiveDepartmentsByFacultyAsync(
        int facultyId)
    {
        return await _context.Set<Department>()
            .AsNoTracking()
            .Where(x =>
                x.FacultyId == facultyId &&
                x.IsActive)
            .OrderBy(x => x.DepartmentName)
            .ToListAsync();
    }

    public async Task<Department?> GetActiveDepartmentByIdAsync(int departmentId)
    {
        return await _context.Set<Department>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.DepartmentId == departmentId &&
                x.IsActive);
    }

    public async Task<IReadOnlyList<University>> GetActiveTreeAsync()
    {
        return await _context.Set<University>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Include(x => x.Faculties.Where(f => f.IsActive))
                .ThenInclude(f => f.Departments.Where(d => d.IsActive))
            .OrderBy(x => x.UniversityName)
            .ToListAsync();
    }
}
