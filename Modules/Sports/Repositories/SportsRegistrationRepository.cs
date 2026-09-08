using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Enums;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Repositories;

public class SportsRegistrationRepository : ISportsRegistrationRepository
{
    private readonly CampusDbContext _context;

    public SportsRegistrationRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<SportsRegistration?> GetByIdAsync(
        int sportsRegistrationId)
    {
        return await _context.SportsRegistrations
            .FirstOrDefaultAsync(x =>
                x.SportsRegistrationId == sportsRegistrationId);
    }

    public async Task<IEnumerable<SportsRegistration>> GetAllAsync()
    {
        return await _context.SportsRegistrations
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<SportsRegistration>>
        GetBySportsEventIdAsync(int sportsEventId)
    {
        return await _context.SportsRegistrations
            .AsNoTracking()
            .Where(x => x.SportsEventId == sportsEventId)
            .ToListAsync();
    }

    public async Task<IEnumerable<SportsRegistration>>
        GetByStudentIdAsync(int studentId)
    {
        return await _context.SportsRegistrations
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .ToListAsync();
    }

    public async Task AddAsync(SportsRegistration registration)
    {
        await _context.SportsRegistrations
            .AddAsync(registration);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SportsRegistration registration)
    {
        _context.SportsRegistrations
            .Update(registration);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int sportsRegistrationId)
    {
        return await _context.SportsRegistrations
            .AnyAsync(x =>
                x.SportsRegistrationId == sportsRegistrationId);
    }

    public async Task<bool> ExistsByEventAndStudentAsync(
        int sportsEventId,
        int studentId)
    {
        return await _context.SportsRegistrations
            .AnyAsync(x =>
                x.SportsEventId == sportsEventId &&
                x.StudentId == studentId);
    }

    public async Task<int> CountByEventAndStudentDepartmentAsync(
        int sportsEventId,
        int departmentId)
    {
        return await _context.SportsRegistrations
            .Where(x =>
                x.SportsEventId == sportsEventId &&
                x.Status != SportsRegistrationStatus.Cancelled)
            .Join(
                _context.Students,
                registration => registration.StudentId,
                student => student.StudentId,
                (registration, student) => student)
            .Join(
                _context.StudentMasterLists,
                student => student.MasterStudentId,
                master => master.MasterStudentId,
                (student, master) => master)
            .CountAsync(master =>
                master.DepartmentId == departmentId);
    }
}