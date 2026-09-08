using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Repositories;

public class SportsEventDepartmentLimitRepository
    : ISportsEventDepartmentLimitRepository
{
    private readonly CampusDbContext _context;

    public SportsEventDepartmentLimitRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<SportsEventDepartmentLimit?> GetByIdAsync(
        int sportsEventDepartmentLimitId)
    {
        return await _context.SportsEventDepartmentLimits
            .FirstOrDefaultAsync(x =>
                x.SportsEventDepartmentLimitId ==
                sportsEventDepartmentLimitId);
    }

    public async Task<IEnumerable<SportsEventDepartmentLimit>>
        GetBySportsEventIdAsync(int sportsEventId)
    {
        return await _context.SportsEventDepartmentLimits
            .AsNoTracking()
            .Where(x => x.SportsEventId == sportsEventId)
            .ToListAsync();
    }

    public async Task AddAsync(
        SportsEventDepartmentLimit departmentLimit)
    {
        await _context.SportsEventDepartmentLimits
            .AddAsync(departmentLimit);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        SportsEventDepartmentLimit departmentLimit)
    {
        _context.SportsEventDepartmentLimits
            .Update(departmentLimit);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(
        int sportsEventDepartmentLimitId)
    {
        return await _context.SportsEventDepartmentLimits
            .AnyAsync(x =>
                x.SportsEventDepartmentLimitId ==
                sportsEventDepartmentLimitId);
    }

    public async Task<bool> ExistsByEventAndDepartmentAsync(
        int sportsEventId,
        int departmentId)
    {
        return await _context.SportsEventDepartmentLimits
            .AnyAsync(x =>
                x.SportsEventId == sportsEventId &&
                x.DepartmentId == departmentId);
    }
}