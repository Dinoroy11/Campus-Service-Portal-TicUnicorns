using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using CanteenEntity = CampusServicePortal_TicUnicorns.Modules.Canteen.Entities.Canteen;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Repositories.Interfaces;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
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
    // CANTEENS
    // =========================================================

    public async Task<List<CanteenEntity>> GetActiveCanteensAsync()
    {
        return await _context.Set<CanteenEntity>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.CanteenName)
            .ToListAsync();
    }

    public async Task<List<CanteenEntity>> GetActiveCanteensByHostelIdAsync(
        int hostelId)
    {
        return await _context.Set<CanteenEntity>()
            .AsNoTracking()
            .Where(x => x.HostelId == hostelId && x.IsActive)
            .OrderBy(x => x.CanteenName)
            .ToListAsync();
    }

    public async Task<CanteenEntity?> GetCanteenByIdAsync(int canteenId)
    {
        return await _context.Set<CanteenEntity>()
            .FirstOrDefaultAsync(x => x.CanteenId == canteenId);
    }

    public async Task<CanteenEntity> AddCanteenAsync(CanteenEntity canteen)
    {
        await _context.Set<CanteenEntity>().AddAsync(canteen);
        await _context.SaveChangesAsync();
        return canteen;
    }

    public async Task<bool> HostelExistsAsync(int hostelId)
    {
        return await _context.Hostels
            .AnyAsync(x => x.HostelId == hostelId && x.IsActive);
    }

    // =========================================================
    // MENU
    // =========================================================

    public async Task<List<CanteenMenuItem>> GetMenuByCanteenIdAsync(
        int canteenId)
    {
        return await _context.Set<CanteenMenuItem>()
            .AsNoTracking()
            .Where(x => x.CanteenId == canteenId && x.IsAvailable)
            .OrderBy(x => x.MealType)
            .ThenBy(x => x.ItemName)
            .ToListAsync();
    }

    public async Task<CanteenMenuItem> AddMenuItemAsync(
        CanteenMenuItem item)
    {
        await _context.Set<CanteenMenuItem>().AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    // =========================================================
    // MEAL PACKAGES / PLANS
    // =========================================================

    public async Task<List<MealPackage>> GetActivePackagesAsync(
        int? canteenId = null)
    {
        var query = _context.MealPackages
            .Include(x => x.Canteen)
            .Where(x => x.IsActive);

        if (canteenId.HasValue)
        {
            query = query.Where(x => x.CanteenId == canteenId.Value);
        }

        return await query
            .OrderBy(x => x.CanteenId)
            .ThenBy(x => x.PlanType)
            .ThenBy(x => x.BillingPeriod)
            .ToListAsync();
    }

    public async Task<MealPackage?> GetPackageByIdAsync(
        int mealPackageId)
    {
        return await _context.MealPackages
            .Include(x => x.Canteen)
            .FirstOrDefaultAsync(x => x.MealPackageId == mealPackageId);
    }

    public async Task<MealPackage?> GetPackageByPlanAsync(
        int canteenId,
        string planType,
        string billingPeriod)
    {
        return await _context.MealPackages
            .FirstOrDefaultAsync(x =>
                x.CanteenId == canteenId &&
                x.PlanType == planType &&
                x.BillingPeriod == billingPeriod);
    }

    public async Task<MealPackage> AddPackageAsync(MealPackage package)
    {
        await _context.MealPackages.AddAsync(package);
        await _context.SaveChangesAsync();
        return package;
    }

    public async Task UpdatePackageAsync(MealPackage package)
    {
        _context.MealPackages.Update(package);
        await _context.SaveChangesAsync();
    }

    // =========================================================
    // STUDENT + HOSTEL ELIGIBILITY
    // =========================================================

    public async Task<Student?> GetStudentByUserIdAsync(int userId)
    {
        return await _context.Students
            .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);
    }

    public async Task<int?> GetActiveHostelIdByStudentIdAsync(int studentId)
    {
        var hostelId = await (
            from allocation in _context.HostelAllocations
            join bed in _context.RoomBeds
                on allocation.BedId equals bed.BedId
            join room in _context.Rooms
                on bed.RoomId equals room.RoomId
            join floor in _context.Floors
                on room.FloorId equals floor.FloorId
            where allocation.StudentId == studentId
                  && allocation.Status == "Active"
            orderby allocation.AllocatedAt descending
            select (int?)floor.HostelId
        ).FirstOrDefaultAsync();

        return hostelId;
    }

    // =========================================================
    // SUBSCRIPTIONS
    // =========================================================

    public async Task<MealSubscription?> GetSubscriptionByIdAsync(
        int mealSubscriptionId)
    {
        return await _context.MealSubscriptions
            .Include(x => x.MealPackage)
                .ThenInclude(x => x!.Canteen)
            .FirstOrDefaultAsync(x =>
                x.MealSubscriptionId == mealSubscriptionId);
    }

    public async Task<MealSubscription?> GetActiveSubscriptionByStudentIdAsync(
        int studentId)
    {
        var today = DateTime.UtcNow.Date;

        return await _context.MealSubscriptions
            .Include(x => x.MealPackage)
             .ThenInclude(x => x!.Canteen)
            .Where(x =>
                x.StudentId == studentId &&
                x.Status == Enums.SubscriptionStatus.Active &&
                x.PaymentStatus == "Paid" &&
                x.StartDate.Date <= today &&
                x.EndDate.Date >= today)
            .OrderByDescending(x => x.EndDate)
            .FirstOrDefaultAsync();
    }

    public async Task<MealSubscription?> GetOverlappingSubscriptionAsync(
        int studentId,
        DateTime startDate,
        DateTime endDate)
    {
        return await _context.MealSubscriptions
            .FirstOrDefaultAsync(x =>
                x.StudentId == studentId &&
                x.Status != Enums.SubscriptionStatus.Cancelled &&
                x.Status != Enums.SubscriptionStatus.Expired &&
                startDate.Date <= x.EndDate.Date &&
                endDate.Date >= x.StartDate.Date);
    }

    public async Task<List<MealSubscription>> GetStudentSubscriptionsAsync(
        int studentId)
    {
        return await _context.MealSubscriptions
            .Include(x => x.MealPackage)
                .ThenInclude(x => x!.Canteen)
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync();
    }

    public async Task<List<MealSubscription>> GetAllSubscriptionsAsync()
    {
        return await _context.MealSubscriptions
            .Include(x => x.MealPackage)
                .ThenInclude(x => x!.Canteen)
            .OrderByDescending(x => x.StartDate)
            .ThenByDescending(x => x.MealSubscriptionId)
            .ToListAsync();
    }

    public async Task<MealSubscription> AddSubscriptionAsync(
        MealSubscription subscription)
    {
        await _context.MealSubscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();
        return subscription;
    }

    public async Task UpdateSubscriptionAsync(MealSubscription subscription)
    {
        _context.MealSubscriptions.Update(subscription);
        await _context.SaveChangesAsync();
    }

    // =========================================================
    // ABSENCE
    // =========================================================

    public async Task<MealAbsence?> GetAbsenceByIdAsync(int mealAbsenceId)
    {
        return await _context.MealAbsences
            .Include(x => x.MealSubscription)
            .FirstOrDefaultAsync(x => x.MealAbsenceId == mealAbsenceId);
    }

    public async Task<List<MealAbsence>> GetAbsencesBySubscriptionIdAsync(
        int mealSubscriptionId)
    {
        return await _context.MealAbsences
            .Where(x => x.MealSubscriptionId == mealSubscriptionId)
            .OrderByDescending(x => x.FromDate)
            .ToListAsync();
    }

    public async Task<MealAbsence> AddAbsenceAsync(MealAbsence absence)
    {
        await _context.MealAbsences.AddAsync(absence);
        await _context.SaveChangesAsync();
        return absence;
    }

    public async Task UpdateAbsenceAsync(MealAbsence absence)
    {
        _context.MealAbsences.Update(absence);
        await _context.SaveChangesAsync();
    }

    // =========================================================
    // USAGE
    // =========================================================

    public async Task<List<MealUsage>> GetUsageBySubscriptionIdAsync(
        int mealSubscriptionId)
    {
        return await _context.MealUsages
            .Where(x => x.MealSubscriptionId == mealSubscriptionId)
            .OrderByDescending(x => x.MealDate)
            .ThenBy(x => x.MealType)
            .ToListAsync();
    }

    public async Task<List<MealUsage>> GetUsageByStudentIdAsync(
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

    public async Task<MealUsage> AddUsageAsync(MealUsage usage)
    {
        await _context.MealUsages.AddAsync(usage);
        await _context.SaveChangesAsync();
        return usage;
    }

    public async Task UpdateUsageAsync(MealUsage usage)
    {
        _context.MealUsages.Update(usage);
        await _context.SaveChangesAsync();
    }
}
