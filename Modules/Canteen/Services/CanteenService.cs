
using CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Enums;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Repositories.Interfaces;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Services.Interfaces;


namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Services;

public class CanteenService : ICanteenService
{
    private readonly ICanteenRepository _repository;

    public CanteenService(ICanteenRepository repository)
    {
        _repository = repository;
    }


    // =========================================================
    // Meal Packages
    // =========================================================

    public async Task<List<MealPackageResponseDto>>
        GetActivePackagesAsync()
    {
        var packages =
            await _repository.GetActivePackagesAsync();

        return packages.Select(x => new MealPackageResponseDto
        {
            MealPackageId = x.MealPackageId,
            PackageCode = x.PackageCode,
            PackageName = x.PackageName,
            BreakfastIncluded = x.BreakfastIncluded,
            LunchIncluded = x.LunchIncluded,
            DinnerIncluded = x.DinnerIncluded,
            MonthlyPrice = x.MonthlyPrice,
            IsActive = x.IsActive
        }).ToList();
    }


    public async Task<MealPackageResponseDto>
        CreatePackageAsync(CreateMealPackageDto dto)
    {
        if (dto.MonthlyPrice <= 0)
            throw new ArgumentException(
                "Monthly price must be greater than zero.");

        if (!dto.BreakfastIncluded &&
            !dto.LunchIncluded &&
            !dto.DinnerIncluded)
        {
            throw new ArgumentException(
                "At least one meal must be included.");
        }

        var package = new MealPackage
        {
            PackageCode = dto.PackageCode.Trim().ToUpper(),
            PackageName = dto.PackageName.Trim(),
            BreakfastIncluded = dto.BreakfastIncluded,
            LunchIncluded = dto.LunchIncluded,
            DinnerIncluded = dto.DinnerIncluded,
            MonthlyPrice = dto.MonthlyPrice,
            IsActive = true
        };

        await _repository.AddPackageAsync(package);

        return new MealPackageResponseDto
        {
            MealPackageId = package.MealPackageId,
            PackageCode = package.PackageCode,
            PackageName = package.PackageName,
            BreakfastIncluded = package.BreakfastIncluded,
            LunchIncluded = package.LunchIncluded,
            DinnerIncluded = package.DinnerIncluded,
            MonthlyPrice = package.MonthlyPrice,
            IsActive = package.IsActive
        };
    }


    // =========================================================
    // Meal Subscription
    // =========================================================

    public async Task<MealSubscriptionResponseDto>
        CreateSubscriptionAsync(
            CreateMealSubscriptionDto dto)
    {
        if (dto.StartDate.Date < DateTime.UtcNow.Date)
        {
            throw new ArgumentException(
                "Subscription cannot start in the past.");
        }

        var existing =
            await _repository
                .GetActiveSubscriptionByStudentIdAsync(
                    dto.StudentId);

        if (existing != null)
        {
            throw new InvalidOperationException(
                "Student already has an active meal subscription.");
        }

        var package =
            await _repository.GetPackageByIdAsync(
                dto.MealPackageId);

        if (package == null || !package.IsActive)
        {
            throw new KeyNotFoundException(
                "Selected meal package was not found or inactive.");
        }

        // One month subscription.
        var endDate =
            dto.StartDate.Date.AddMonths(1).AddDays(-1);

        var subscription = new MealSubscription
        {
            StudentId = dto.StudentId,
            MealPackageId = package.MealPackageId,
            StartDate = dto.StartDate.Date,
            EndDate = endDate,
            Amount = package.MonthlyPrice,
            Status = SubscriptionStatus.Pending
        };

        await _repository.AddSubscriptionAsync(
            subscription);

        return MapSubscription(
            subscription,
            package);
    }


    public async Task<MealSubscriptionResponseDto?>
        GetActiveSubscriptionAsync(int studentId)
    {
        var subscription =
            await _repository
                .GetActiveSubscriptionByStudentIdAsync(
                    studentId);

        if (subscription == null)
            return null;

        return MapSubscription(
            subscription,
            subscription.MealPackage!);
    }


    public async Task<List<MealSubscriptionResponseDto>>
        GetStudentSubscriptionsAsync(int studentId)
    {
        var subscriptions =
            await _repository
                .GetStudentSubscriptionsAsync(studentId);

        return subscriptions.Select(x =>
            MapSubscription(
                x,
                x.MealPackage!))
            .ToList();
    }


    // =========================================================
    // Absence
    // =========================================================

    public async Task ReportAbsenceAsync(
        ReportMealAbsenceDto dto)
    {
        if (dto.FromDate.Date > dto.ToDate.Date)
        {
            throw new ArgumentException(
                "From date cannot be after to date.");
        }

        var subscription =
            await _repository.GetSubscriptionByIdAsync(
                dto.MealSubscriptionId);

        if (subscription == null)
        {
            throw new KeyNotFoundException(
                "Meal subscription not found.");
        }

        if (dto.FromDate.Date < subscription.StartDate.Date ||
            dto.ToDate.Date > subscription.EndDate.Date)
        {
            throw new ArgumentException(
                "Absence period must be within the subscription period.");
        }

        var existingAbsences =
            await _repository
                .GetAbsencesBySubscriptionIdAsync(
                    dto.MealSubscriptionId);

        var overlaps = existingAbsences.Any(x =>
            dto.FromDate.Date <= x.ToDate.Date &&
            dto.ToDate.Date >= x.FromDate.Date &&
            x.Status != AbsenceStatus.Rejected);

        if (overlaps)
        {
            throw new InvalidOperationException(
                "The reported absence period overlaps an existing absence.");
        }

        var eligibleDays =
            (dto.ToDate.Date - dto.FromDate.Date).Days + 1;

        var absence = new MealAbsence
        {
            MealSubscriptionId = dto.MealSubscriptionId,
            FromDate = dto.FromDate.Date,
            ToDate = dto.ToDate.Date,
            Reason = dto.Reason?.Trim(),
            EligibleDays = eligibleDays,
            Status = AbsenceStatus.Pending
        };

        await _repository.AddAbsenceAsync(absence);
    }


    public async Task<List<MealAbsence>> GetAbsencesAsync(
        int mealSubscriptionId)
    {
        return await _repository
            .GetAbsencesBySubscriptionIdAsync(
                mealSubscriptionId);
    }


    // =========================================================
    // Meal Usage
    // =========================================================

    public async Task<List<MealUsageResponseDto>>
        GetStudentMealUsageAsync(
            int studentId,
            DateTime? fromDate = null,
            DateTime? toDate = null)
    {
        var usage =
            await _repository.GetUsageByStudentIdAsync(
                studentId,
                fromDate,
                toDate);

        return usage.Select(x =>
            new MealUsageResponseDto
            {
                MealUsageId = x.MealUsageId,
                StudentId = x.StudentId,
                MealDate = x.MealDate,
                MealType = x.MealType,
                Status = x.Status,
                CollectedAt = x.CollectedAt
            }).ToList();
    }


    public async Task<MealUsageResponseDto>
        CollectMealAsync(
            int mealSubscriptionId,
            DateTime mealDate,
            MealType mealType)
    {
        var subscription =
            await _repository.GetSubscriptionByIdAsync(
                mealSubscriptionId);

        if (subscription == null)
        {
            throw new KeyNotFoundException(
                "Meal subscription not found.");
        }

        if (subscription.Status !=
            SubscriptionStatus.Active)
        {
            throw new InvalidOperationException(
                "Meal subscription is not active.");
        }

        if (mealDate.Date < subscription.StartDate.Date ||
            mealDate.Date > subscription.EndDate.Date)
        {
            throw new ArgumentException(
                "Meal date is outside the subscription period.");
        }

        var package = subscription.MealPackage;

        if (!IsMealIncluded(package!, mealType))
        {
            throw new InvalidOperationException(
                "This meal is not included in the student's package.");
        }

        var existing =
            await _repository.GetUsageAsync(
                mealSubscriptionId,
                mealDate,
                mealType.ToString());

        if (existing != null &&
            existing.Status == MealUsageStatus.Collected)
        {
            throw new InvalidOperationException(
                "This meal has already been collected.");
        }

        var usage = existing ?? new MealUsage
        {
            MealSubscriptionId =
                mealSubscriptionId,

            StudentId =
                subscription.StudentId,

            MealDate =
                mealDate.Date,

            MealType =
                mealType
        };

        usage.Status = MealUsageStatus.Collected;
        usage.CollectedAt = DateTime.UtcNow;

        if (existing == null)
        {
            await _repository.AddUsageAsync(usage);
        }
        else
        {
            await _repository.UpdateUsageAsync(usage);
        }

        return new MealUsageResponseDto
        {
            MealUsageId = usage.MealUsageId,
            StudentId = usage.StudentId,
            MealDate = usage.MealDate,
            MealType = usage.MealType,
            Status = usage.Status,
            CollectedAt = usage.CollectedAt
        };
    }


    // =========================================================
    // Helpers
    // =========================================================

    private static bool IsMealIncluded(
        MealPackage package,
        MealType mealType)
    {
        return mealType switch
        {
            MealType.Breakfast =>
                package.BreakfastIncluded,

            MealType.Lunch =>
                package.LunchIncluded,

            MealType.Dinner =>
                package.DinnerIncluded,

            _ => false
        };
    }


    private static MealSubscriptionResponseDto
        MapSubscription(
            MealSubscription subscription,
            MealPackage package)
    {
        return new MealSubscriptionResponseDto
        {
            MealSubscriptionId =
                subscription.MealSubscriptionId,

            StudentId =
                subscription.StudentId,

            MealPackageId =
                subscription.MealPackageId,

            PackageCode =
                package.PackageCode,

            PackageName =
                package.PackageName,

            StartDate =
                subscription.StartDate,

            EndDate =
                subscription.EndDate,

            Amount =
                subscription.Amount,

            StudentFeeId =
                subscription.StudentFeeId,

            Status =
                subscription.Status,

            UnusedDays = 0,

            CarryForwardDays = 0
        };
    }
}
