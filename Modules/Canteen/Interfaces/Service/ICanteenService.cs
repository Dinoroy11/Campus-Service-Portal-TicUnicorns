
using CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Enums;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Services.Interfaces;

public interface ICanteenService
{
    // =========================================================
    // Meal Packages
    // =========================================================

    Task<List<MealPackageResponseDto>> GetActivePackagesAsync();

    Task<MealPackageResponseDto> CreatePackageAsync(
        CreateMealPackageDto dto);


    // =========================================================
    // Meal Subscription
    // =========================================================

    Task<MealSubscriptionResponseDto> CreateSubscriptionAsync(
        CreateMealSubscriptionDto dto);

    Task<MealSubscriptionResponseDto?>
        GetActiveSubscriptionAsync(int studentId);

    Task<List<MealSubscriptionResponseDto>>
        GetStudentSubscriptionsAsync(int studentId);


    // =========================================================
    // Absence
    // =========================================================

    Task ReportAbsenceAsync(
        ReportMealAbsenceDto dto);

    Task<List<MealAbsence>> GetAbsencesAsync(
        int mealSubscriptionId);


    // =========================================================
    // Meal Usage
    // =========================================================

    Task<List<MealUsageResponseDto>>
        GetStudentMealUsageAsync(
            int studentId,
            DateTime? fromDate = null,
            DateTime? toDate = null);

    Task<MealUsageResponseDto>
        CollectMealAsync(
            int mealSubscriptionId,
            DateTime mealDate,
            MealType mealType);
}
