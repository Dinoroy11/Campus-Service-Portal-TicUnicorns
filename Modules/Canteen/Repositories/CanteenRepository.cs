using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;



namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Repositories;

public class CanteenRepository : ICanteenRepository
{
    private readonly CampusDbContext _context;

    public CanteenRepository(CampusDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // Meal Packages
    // =========================================================

    public async Task<List<MealPackage>> GetActivePackagesAsync()
    {
        return await _context.MealPackages
            .Where(x => x.IsActive)
            .OrderBy(x => x.MonthlyPrice)
            .ToListAsync();
    }

    public async Task<MealPackage?> GetPackageByIdAsync(
        int mealPackageId)
    {
        return await _context.MealPackages
            .FirstOrDefaultAsync(x =>
                x.MealPackageId == mealPackageId);
    }

    public async Task<MealPackage> AddPackageAsync(
        MealPackage package)
    {
        await _context.MealPackages.AddAsync(package);
        await _context.SaveChangesAsync();

        return package;
    }

    public async Task UpdatePackageAsync(
        MealPackage package)
    {
        _context.MealPackages.Update(package);
        await _context.SaveChangesAsync();
    }


    // =========================================================
    // Meal Subscriptions
    // =========================================================

    public async Task<MealSubscription?> GetSubscriptionByIdAsync(
        int mealSubscriptionId)
    {
        return await _context.MealSubscriptions
            .Include(x => x.MealPackage)
            .FirstOrDefaultAsync(x =>
                x.MealSubscriptionId == mealSubscriptionId);
    }

    public async Task<MealSubscription?>
        GetActiveSubscriptionByStudentIdAsync(int studentId)
    {
        var today = DateTime.UtcNow.Date;

        return await _context.MealSubscriptions
            .Include(x => x.MealPackage)
            .Where(x =>
                x.StudentId == studentId &&
                x.Status == Enums.SubscriptionStatus.Active &&
                x.StartDate.Date <= today &&
                x.EndDate.Date >= today)
            .OrderByDescending(x => x.EndDate)
            .FirstOrDefaultAsync();
    }

    public async Task<List<MealSubscription>>
        GetStudentSubscriptionsAsync(int studentId)
    {
        return await _context.MealSubscriptions
            .Include(x => x.MealPackage)
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync();
    }

    public async Task<MealSubscription> AddSubscriptionAsync(
        MealSubscription subscription)
    {
        await _context.MealSubscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        return subscription;
    }

    public async Task UpdateSubscriptionAsync(
        MealSubscription subscription)
    {
        _context.MealSubscriptions.Update(subscription);
        await _context.SaveChangesAsync();
    }


    // =========================================================
    // Meal Absence
    // =========================================================

    public async Task<MealAbsence?> GetAbsenceByIdAsync(
        int mealAbsenceId)
    {
        return await _context.MealAbsences
            .Include(x => x.MealSubscription)
            .FirstOrDefaultAsync(x =>
                x.MealAbsenceId == mealAbsenceId);
    }

    public async Task<List<MealAbsence>>
        GetAbsencesBySubscriptionIdAsync(
            int mealSubscriptionId)
    {
        return await _context.MealAbsences
            .Where(x =>
                x.MealSubscriptionId == mealSubscriptionId)
            .OrderByDescending(x => x.FromDate)
            .ToListAsync();
    }

    public async Task<MealAbsence> AddAbsenceAsync(
        MealAbsence absence)
    {
        await _context.MealAbsences.AddAsync(absence);
        await _context.SaveChangesAsync();

        return absence;
    }

    public async Task UpdateAbsenceAsync(
        MealAbsence absence)
    {
        _context.MealAbsences.Update(absence);
        await _context.SaveChangesAsync();
    }


    // =========================================================
    // Meal Usage
    // =========================================================

    public async Task<List<MealUsage>>
        GetUsageBySubscriptionIdAsync(
            int mealSubscriptionId)
    {
        return await _context.MealUsages
            .Where(x =>
                x.MealSubscriptionId == mealSubscriptionId)
            .OrderByDescending(x => x.MealDate)
            .ThenBy(x => x.MealType)
            .ToListAsync();
    }

    public async Task<List<MealUsage>>
        GetUsageByStudentIdAsync(
            int studentId,
            DateTime? fromDate = null,
            DateTime? toDate = null)
    {
        var query = _context.MealUsages
            .Where(x => x.StudentId == studentId);

        if (fromDate.HasValue)
        {
            query = query.Where(x =>
                x.MealDate.Date >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x =>
                x.MealDate.Date <= toDate.Value.Date);
        }

        return await query
            .OrderByDescending(x => x.MealDate)
            .ThenBy(x => x.MealType)
            .ToListAsync();
    }

    public async Task<MealUsage?> GetUsageAsync(
        int mealSubscriptionId,
        DateTime mealDate,
        string mealType)
    {
        return await _context.MealUsages
            .FirstOrDefaultAsync(x =>
                x.MealSubscriptionId == mealSubscriptionId &&
                x.MealDate.Date == mealDate.Date &&
                x.MealType.ToString() == mealType);
    }

    public async Task<MealUsage> AddUsageAsync(
        MealUsage usage)
    {
        await _context.MealUsages.AddAsync(usage);
        await _context.SaveChangesAsync();

        return usage;
    }

    public async Task UpdateUsageAsync(
        MealUsage usage)
    {
        _context.MealUsages.Update(usage);
        await _context.SaveChangesAsync();
    }
}