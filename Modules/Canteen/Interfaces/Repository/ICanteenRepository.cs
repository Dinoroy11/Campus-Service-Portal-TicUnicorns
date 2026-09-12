using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using CanteenEntity = CampusServicePortal_TicUnicorns.Modules.Canteen.Entities.Canteen;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Repositories.Interfaces;

public interface ICanteenRepository
{
    // Canteens
    Task<List<CanteenEntity>> GetActiveCanteensAsync();
    Task<List<CanteenEntity>> GetActiveCanteensByHostelIdAsync(int hostelId);
    Task<CanteenEntity?> GetCanteenByIdAsync(int canteenId);
    Task<CanteenEntity> AddCanteenAsync(CanteenEntity canteen);
    Task<bool> HostelExistsAsync(int hostelId);

    // Menu
    Task<List<CanteenMenuItem>> GetMenuByCanteenIdAsync(int canteenId);
    Task<CanteenMenuItem> AddMenuItemAsync(CanteenMenuItem item);

    // Meal Packages / Plans
    Task<List<MealPackage>> GetActivePackagesAsync(int? canteenId = null);
    Task<MealPackage?> GetPackageByIdAsync(int mealPackageId);
    Task<MealPackage?> GetPackageByPlanAsync(
        int canteenId,
        string planType,
        string billingPeriod);
    Task<MealPackage> AddPackageAsync(MealPackage package);
    Task UpdatePackageAsync(MealPackage package);

    // Student + hostel eligibility
    Task<Student?> GetStudentByUserIdAsync(int userId);
    Task<int?> GetActiveHostelIdByStudentIdAsync(int studentId);

    // Meal Subscriptions
    Task<MealSubscription?> GetSubscriptionByIdAsync(int mealSubscriptionId);
    Task<MealSubscription?> GetActiveSubscriptionByStudentIdAsync(int studentId);
    Task<MealSubscription?> GetOverlappingSubscriptionAsync(
        int studentId,
        DateTime startDate,
        DateTime endDate);
    Task<List<MealSubscription>> GetStudentSubscriptionsAsync(int studentId);
    Task<MealSubscription> AddSubscriptionAsync(MealSubscription subscription);
    Task UpdateSubscriptionAsync(MealSubscription subscription);

    // Meal Absence
    Task<MealAbsence?> GetAbsenceByIdAsync(int mealAbsenceId);
    Task<List<MealAbsence>> GetAbsencesBySubscriptionIdAsync(int mealSubscriptionId);
    Task<MealAbsence> AddAbsenceAsync(MealAbsence absence);
    Task UpdateAbsenceAsync(MealAbsence absence);

    // Meal Usage
    Task<List<MealUsage>> GetUsageBySubscriptionIdAsync(int mealSubscriptionId);
    Task<List<MealUsage>> GetUsageByStudentIdAsync(
        int studentId,
        DateTime? fromDate = null,
        DateTime? toDate = null);
    Task<MealUsage?> GetUsageAsync(
        int mealSubscriptionId,
        DateTime mealDate,
        string mealType);
    Task<MealUsage> AddUsageAsync(MealUsage usage);
    Task UpdateUsageAsync(MealUsage usage);
}
