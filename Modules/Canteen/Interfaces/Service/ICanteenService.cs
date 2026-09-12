using CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Enums;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Services.Interfaces;

public interface ICanteenService
{
    // Canteens
    Task<List<CanteenDto>> GetCanteensAsync();
    Task<List<CanteenDto>> GetMyCanteensAsync(int userId);
    Task<CanteenDto> CreateCanteenAsync(CreateCanteenDto dto);

    // Menu
    Task<List<CanteenMenuItemDto>> GetMenuAsync(int canteenId);
    Task<CanteenMenuItemDto> CreateMenuItemAsync(CreateCanteenMenuItemDto dto);

    // Meal Plans
    Task<List<MealPackageResponseDto>> GetActivePackagesAsync(int? canteenId = null);
    Task<MealPackageResponseDto> CreatePackageAsync(CreateMealPackageDto dto);

    // Subscription + payment
    Task<MealSubscriptionResponseDto> CreateSubscriptionAsync(
        int userId,
        CreateMealSubscriptionDto dto);
    Task<MealSubscriptionResponseDto> PaySubscriptionAsync(
        int userId,
        int mealSubscriptionId,
        SimulateMealSubscriptionPaymentDto dto);
    Task<MealSubscriptionResponseDto?> GetMyActiveSubscriptionAsync(int userId);
    Task<List<MealSubscriptionResponseDto>> GetMySubscriptionsAsync(int userId);
    Task<List<MealSubscriptionResponseDto>> GetStudentSubscriptionsAsync(int studentId);

    // Absence
    Task ReportAbsenceAsync(int userId, ReportMealAbsenceDto dto);
    Task<List<MealAbsence>> GetAbsencesAsync(int userId, int mealSubscriptionId);

    // Usage
    Task<List<MealUsageResponseDto>> GetMyMealUsageAsync(
        int userId,
        DateTime? fromDate = null,
        DateTime? toDate = null);
    Task<MealUsageResponseDto> CollectMealAsync(
        int mealSubscriptionId,
        DateTime mealDate,
        MealType mealType);
}
