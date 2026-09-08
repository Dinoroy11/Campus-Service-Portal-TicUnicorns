
using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Repositories.Interfaces;

public interface ICanteenRepository
{
    // =========================================================
    // Meal Packages
    // =========================================================

    Task<List<MealPackage>> GetActivePackagesAsync();

    Task<MealPackage?> GetPackageByIdAsync(int mealPackageId);

    Task<MealPackage> AddPackageAsync(MealPackage package);

    Task UpdatePackageAsync(MealPackage package);


    // =========================================================
    // Meal Subscriptions
    // =========================================================

    Task<MealSubscription?> GetSubscriptionByIdAsync(
        int mealSubscriptionId);

    Task<MealSubscription?> GetActiveSubscriptionByStudentIdAsync(
        int studentId);

    Task<List<MealSubscription>> GetStudentSubscriptionsAsync(
        int studentId);

    Task<MealSubscription> AddSubscriptionAsync(
        MealSubscription subscription);

    Task UpdateSubscriptionAsync(
        MealSubscription subscription);


    // =========================================================
    // Meal Absence
    // =========================================================

    Task<MealAbsence?> GetAbsenceByIdAsync(
        int mealAbsenceId);

    Task<List<MealAbsence>> GetAbsencesBySubscriptionIdAsync(
        int mealSubscriptionId);

    Task<MealAbsence> AddAbsenceAsync(
        MealAbsence absence);

    Task UpdateAbsenceAsync(
        MealAbsence absence);


    // =========================================================
    // Meal Usage
    // =========================================================

    Task<List<MealUsage>> GetUsageBySubscriptionIdAsync(
        int mealSubscriptionId);

    Task<List<MealUsage>> GetUsageByStudentIdAsync(
        int studentId,
        DateTime? fromDate = null,
        DateTime? toDate = null);

    Task<MealUsage?> GetUsageAsync(
        int mealSubscriptionId,
        DateTime mealDate,
        string mealType);

    Task<MealUsage> AddUsageAsync(
        MealUsage usage);

    Task UpdateUsageAsync(
        MealUsage usage);
}
