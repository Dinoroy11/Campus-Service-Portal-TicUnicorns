using CampusServicePortal.Modules.Notifications.DTOs;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using CanteenEntity = CampusServicePortal_TicUnicorns.Modules.Canteen.Entities.Canteen;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Enums;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Repositories.Interfaces;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Services.Interfaces;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Services;

public class CanteenService : ICanteenService
{
    private readonly ICanteenRepository _repository;
    private readonly INotificationService _notificationService;

    public CanteenService(
        ICanteenRepository repository,
        INotificationService notificationService)
    {
        _repository = repository;
        _notificationService = notificationService;
    }

    // =========================================================
    // CANTEENS
    // =========================================================

    public async Task<List<CanteenDto>> GetCanteensAsync()
    {
        var canteens = await _repository.GetActiveCanteensAsync();
        return canteens.Select(MapCanteen).ToList();
    }

    public async Task<List<CanteenDto>> GetMyCanteensAsync(int userId)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var hostelId = await _repository
            .GetActiveHostelIdByStudentIdAsync(student.StudentId);

        if (!hostelId.HasValue)
        {
            throw new InvalidOperationException(
                "Only students with an active hostel allocation can use hostel meal plans.");
        }

        var canteens = await _repository
            .GetActiveCanteensByHostelIdAsync(hostelId.Value);

        return canteens.Select(MapCanteen).ToList();
    }

    public async Task<CanteenDto> CreateCanteenAsync(CreateCanteenDto dto)
    {
        if (dto.HostelId <= 0)
        {
            throw new ArgumentException("A valid HostelId is required.");
        }

        if (!await _repository.HostelExistsAsync(dto.HostelId))
        {
            throw new KeyNotFoundException(
                "The selected hostel was not found or is inactive.");
        }

        if (string.IsNullOrWhiteSpace(dto.CanteenName))
        {
            throw new ArgumentException("Canteen name is required.");
        }

        var canteen = new CanteenEntity
        {
            HostelId = dto.HostelId,
            CanteenName = dto.CanteenName.Trim(),
            Description = dto.Description?.Trim(),
            IsActive = true
        };

        await _repository.AddCanteenAsync(canteen);
        return MapCanteen(canteen);
    }

    // =========================================================
    // MENU
    // =========================================================

    public async Task<List<CanteenMenuItemDto>> GetMenuAsync(int canteenId)
    {
        var canteen = await _repository.GetCanteenByIdAsync(canteenId);
        if (canteen == null || !canteen.IsActive)
        {
            throw new KeyNotFoundException("Canteen was not found or is inactive.");
        }

        var items = await _repository.GetMenuByCanteenIdAsync(canteenId);
        return items.Select(MapMenuItem).ToList();
    }

    public async Task<CanteenMenuItemDto> CreateMenuItemAsync(
        CreateCanteenMenuItemDto dto)
    {
        var canteen = await _repository.GetCanteenByIdAsync(dto.CanteenId);
        if (canteen == null || !canteen.IsActive)
        {
            throw new KeyNotFoundException("Canteen was not found or is inactive.");
        }

        if (string.IsNullOrWhiteSpace(dto.ItemName))
        {
            throw new ArgumentException("Menu item name is required.");
        }

        var item = new CanteenMenuItem
        {
            CanteenId = dto.CanteenId,
            ItemName = dto.ItemName.Trim(),
            Description = dto.Description?.Trim(),
            MealType = dto.MealType,
            Price = dto.Price,
            IsAvailable = true
        };

        await _repository.AddMenuItemAsync(item);
        return MapMenuItem(item);
    }

    // =========================================================
    // MEAL PLANS
    // =========================================================

    public async Task<List<MealPackageResponseDto>> GetActivePackagesAsync(
        int? canteenId = null)
    {
        var packages = await _repository.GetActivePackagesAsync(canteenId);
        return packages.Select(MapPackage).ToList();
    }

    public async Task<MealPackageResponseDto> CreatePackageAsync(
        CreateMealPackageDto dto)
    {
        var canteen = await _repository.GetCanteenByIdAsync(dto.CanteenId);
        if (canteen == null || !canteen.IsActive)
        {
            throw new KeyNotFoundException("Canteen was not found or is inactive.");
        }

        if (dto.Price <= 0)
        {
            throw new ArgumentException("Plan price must be greater than zero.");
        }

        var planType = NormalizePlanType(dto.PlanType);
        var billingPeriod = NormalizeBillingPeriod(dto.BillingPeriod);

        var existing = await _repository.GetPackageByPlanAsync(
            dto.CanteenId,
            planType,
            billingPeriod);

        if (existing != null)
        {
            throw new InvalidOperationException(
                $"A {planType} {billingPeriod} plan already exists for this canteen.");
        }

        var package = new MealPackage
        {
            CanteenId = dto.CanteenId,
            PackageCode = $"{planType}-{billingPeriod.ToUpperInvariant()}-C{dto.CanteenId}",
            PackageName = $"{planType} {billingPeriod} Meal Plan",
            PlanType = planType,
            BillingPeriod = billingPeriod,
            MonthlyPrice = dto.Price,
            IsActive = true
        };

        ApplyMealsForPlan(package, planType);

        await _repository.AddPackageAsync(package);
        package.Canteen = canteen;

        return MapPackage(package);
    }

    // =========================================================
    // SUBSCRIPTION
    // =========================================================

    public async Task<MealSubscriptionResponseDto> CreateSubscriptionAsync(
        int userId,
        CreateMealSubscriptionDto dto)
    {
        var student = await GetStudentByUserIdAsync(userId);

        if (dto.StartDate.Date < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Subscription cannot start in the past.");
        }

        var hostelId = await _repository
            .GetActiveHostelIdByStudentIdAsync(student.StudentId);

        if (!hostelId.HasValue)
        {
            throw new InvalidOperationException(
                "Only students with an active hostel allocation can subscribe to a hostel meal plan.");
        }

        var package = await _repository.GetPackageByIdAsync(dto.MealPackageId);
        if (package == null || !package.IsActive)
        {
            throw new KeyNotFoundException(
                "Selected meal plan was not found or is inactive.");
        }

        var packageCanteen = package.Canteen;
        if (packageCanteen == null || !packageCanteen.IsActive)
        {
            throw new KeyNotFoundException(
                "The meal plan canteen was not found or is inactive.");
        }

        if (packageCanteen.HostelId != hostelId.Value)
        {
            throw new InvalidOperationException(
                "You can subscribe only to a canteen belonging to your allocated hostel.");
        }

        var endDate = CalculateEndDate(dto.StartDate.Date, package.BillingPeriod);

        var overlapping = await _repository.GetOverlappingSubscriptionAsync(
            student.StudentId,
            dto.StartDate.Date,
            endDate);

        if (overlapping != null)
        {
            throw new InvalidOperationException(
                "An overlapping meal subscription already exists for this student.");
        }

        var subscription = new MealSubscription
        {
            StudentId = student.StudentId,
            MealPackageId = package.MealPackageId,
            StartDate = dto.StartDate.Date,
            EndDate = endDate,
            Amount = package.MonthlyPrice,
            Status = SubscriptionStatus.Pending,
            PaymentStatus = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddSubscriptionAsync(subscription);
        subscription.MealPackage = package;

        return MapSubscription(subscription, package);
    }

    public async Task<MealSubscriptionResponseDto> PaySubscriptionAsync(
        int userId,
        int mealSubscriptionId,
        SimulateMealSubscriptionPaymentDto dto)
    {
        var student = await GetStudentByUserIdAsync(userId);

        var subscription = await _repository
            .GetSubscriptionByIdAsync(mealSubscriptionId);

        if (subscription == null)
        {
            throw new KeyNotFoundException("Meal subscription was not found.");
        }

        if (subscription.StudentId != student.StudentId)
        {
            throw new UnauthorizedAccessException(
                "You cannot pay another student's meal subscription.");
        }

        if (subscription.PaymentStatus.Equals(
                "Paid",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "This meal subscription has already been paid.");
        }

        if (subscription.Status == SubscriptionStatus.Cancelled ||
            subscription.Status == SubscriptionStatus.Expired ||
            subscription.EndDate.Date < DateTime.UtcNow.Date)
        {
            subscription.Status = SubscriptionStatus.Expired;
            await _repository.UpdateSubscriptionAsync(subscription);

            throw new InvalidOperationException(
                "This meal subscription is no longer payable.");
        }

        subscription.PaymentStatus = "Paid";
        subscription.PaymentReference = string.IsNullOrWhiteSpace(dto.PaymentReference)
            ? $"SIM-CAN-{DateTime.UtcNow:yyyyMMddHHmmss}-{subscription.MealSubscriptionId}"
            : dto.PaymentReference.Trim();
        subscription.PaidAt = DateTime.UtcNow;
        subscription.Status = SubscriptionStatus.Active;

        await _repository.UpdateSubscriptionAsync(subscription);

        if (student.UserId.HasValue)
        {
            await _notificationService.CreateAsync(
                new NotificationCreateDto
                {
                    UserId = student.UserId.Value,
                    Title = "Meal Plan Subscription Activated",
                    Message =
                        $"Your {subscription.MealPackage?.PackageName ?? "meal plan"} subscription " +
                        $"is active from {subscription.StartDate:yyyy-MM-dd} to {subscription.EndDate:yyyy-MM-dd}.",
                    ReferenceType = "MealSubscription",
                    ReferenceId = subscription.MealSubscriptionId
                });
        }

        var mealPackage = subscription.MealPackage
            ?? throw new InvalidOperationException("Meal package data is missing for this subscription.");

        return MapSubscription(
            subscription,
            mealPackage);
    }

    public async Task<MealSubscriptionResponseDto?> GetMyActiveSubscriptionAsync(
        int userId)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var subscription = await _repository
            .GetActiveSubscriptionByStudentIdAsync(student.StudentId);

        if (subscription == null)
        {
            return null;
        }

        var mealPackage = subscription.MealPackage
            ?? throw new InvalidOperationException("Meal package data is missing for this subscription.");

        return MapSubscription(subscription, mealPackage);
    }

    public async Task<List<MealSubscriptionResponseDto>> GetMySubscriptionsAsync(
        int userId)
    {
        var student = await GetStudentByUserIdAsync(userId);
        return await GetStudentSubscriptionsAsync(student.StudentId);
    }

    public async Task<List<MealSubscriptionResponseDto>> GetStudentSubscriptionsAsync(
        int studentId)
    {
        var subscriptions = await _repository.GetStudentSubscriptionsAsync(studentId);
        return subscriptions
            .Where(x => x.MealPackage != null)
            .Select(x => MapSubscription(x, x.MealPackage!))
            .ToList();
    }

    // =========================================================
    // ABSENCE
    // =========================================================

    public async Task ReportAbsenceAsync(
        int userId,
        ReportMealAbsenceDto dto)
    {
        var student = await GetStudentByUserIdAsync(userId);

        if (dto.FromDate.Date > dto.ToDate.Date)
        {
            throw new ArgumentException("From date cannot be after to date.");
        }

        var subscription = await _repository
            .GetSubscriptionByIdAsync(dto.MealSubscriptionId);

        if (subscription == null)
        {
            throw new KeyNotFoundException("Meal subscription not found.");
        }

        EnsureSubscriptionOwner(subscription, student.StudentId);

        if (subscription.Status != SubscriptionStatus.Active ||
            !subscription.PaymentStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Only an active paid meal subscription can report an absence.");
        }

        if (dto.FromDate.Date < subscription.StartDate.Date ||
            dto.ToDate.Date > subscription.EndDate.Date)
        {
            throw new ArgumentException(
                "Absence period must be within the subscription period.");
        }

        var existingAbsences = await _repository
            .GetAbsencesBySubscriptionIdAsync(dto.MealSubscriptionId);

        var overlaps = existingAbsences.Any(x =>
            dto.FromDate.Date <= x.ToDate.Date &&
            dto.ToDate.Date >= x.FromDate.Date &&
            x.Status != AbsenceStatus.Rejected);

        if (overlaps)
        {
            throw new InvalidOperationException(
                "The reported absence period overlaps an existing absence.");
        }

        var absence = new MealAbsence
        {
            MealSubscriptionId = dto.MealSubscriptionId,
            FromDate = dto.FromDate.Date,
            ToDate = dto.ToDate.Date,
            Reason = dto.Reason?.Trim(),
            EligibleDays = (dto.ToDate.Date - dto.FromDate.Date).Days + 1,
            Status = AbsenceStatus.Pending
        };

        await _repository.AddAbsenceAsync(absence);
    }

    public async Task<List<MealAbsence>> GetAbsencesAsync(
        int userId,
        int mealSubscriptionId)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var subscription = await _repository.GetSubscriptionByIdAsync(mealSubscriptionId);

        if (subscription == null)
        {
            throw new KeyNotFoundException("Meal subscription not found.");
        }

        EnsureSubscriptionOwner(subscription, student.StudentId);

        return await _repository.GetAbsencesBySubscriptionIdAsync(mealSubscriptionId);
    }

    // =========================================================
    // MEAL USAGE
    // =========================================================

    public async Task<List<MealUsageResponseDto>> GetMyMealUsageAsync(
        int userId,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var student = await GetStudentByUserIdAsync(userId);

        var usage = await _repository.GetUsageByStudentIdAsync(
            student.StudentId,
            fromDate,
            toDate);

        return usage.Select(MapUsage).ToList();
    }

    public async Task<MealUsageResponseDto> CollectMealAsync(
        int mealSubscriptionId,
        DateTime mealDate,
        MealType mealType)
    {
        var subscription = await _repository.GetSubscriptionByIdAsync(mealSubscriptionId);

        if (subscription == null)
        {
            throw new KeyNotFoundException("Meal subscription not found.");
        }

        if (subscription.Status != SubscriptionStatus.Active ||
            !subscription.PaymentStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Meal subscription is not active and paid.");
        }

        if (mealDate.Date < subscription.StartDate.Date ||
            mealDate.Date > subscription.EndDate.Date)
        {
            throw new ArgumentException("Meal date is outside the subscription period.");
        }

        var package = subscription.MealPackage;
        if (package == null || !IsMealIncluded(package, mealType))
        {
            throw new InvalidOperationException(
                "This meal is not included in the student's plan.");
        }

        var existing = await _repository.GetUsageAsync(
            mealSubscriptionId,
            mealDate,
            mealType.ToString());

        if (existing != null && existing.Status == MealUsageStatus.Collected)
        {
            throw new InvalidOperationException("This meal has already been collected.");
        }

        var usage = existing ?? new MealUsage
        {
            MealSubscriptionId = mealSubscriptionId,
            StudentId = subscription.StudentId,
            MealDate = mealDate.Date,
            MealType = mealType
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

        return MapUsage(usage);
    }

    // =========================================================
    // HELPERS
    // =========================================================

    private async Task<Student> GetStudentByUserIdAsync(
        int userId)
    {
        var student = await _repository.GetStudentByUserIdAsync(userId);
        if (student == null)
        {
            throw new UnauthorizedAccessException(
                "A student profile is required for this operation.");
        }

        return student;
    }

    private static string NormalizePlanType(string value)
    {
        var normalized = value?.Trim().ToUpperInvariant();
        if (normalized is not ("BB" or "HB" or "FB"))
        {
            throw new ArgumentException("PlanType must be BB, HB or FB.");
        }

        return normalized;
    }

    private static string NormalizeBillingPeriod(string value)
    {
        var normalized = value?.Trim();

        if (string.Equals(normalized, "Weekly", StringComparison.OrdinalIgnoreCase))
        {
            return "Weekly";
        }

        if (string.Equals(normalized, "Monthly", StringComparison.OrdinalIgnoreCase))
        {
            return "Monthly";
        }

        throw new ArgumentException("BillingPeriod must be Weekly or Monthly.");
    }

    private static void ApplyMealsForPlan(MealPackage package, string planType)
    {
        package.BreakfastIncluded = true;
        package.LunchIncluded = planType == "FB";
        package.DinnerIncluded = planType is "HB" or "FB";
    }

    private static DateTime CalculateEndDate(DateTime startDate, string billingPeriod)
    {
        return billingPeriod.Equals("Weekly", StringComparison.OrdinalIgnoreCase)
            ? startDate.AddDays(6)
            : startDate.AddMonths(1).AddDays(-1);
    }

    private static void EnsureSubscriptionOwner(
        MealSubscription subscription,
        int studentId)
    {
        if (subscription.StudentId != studentId)
        {
            throw new UnauthorizedAccessException(
                "You cannot access another student's meal subscription.");
        }
    }

    private static bool IsMealIncluded(MealPackage package, MealType mealType)
    {
        return mealType switch
        {
            MealType.Breakfast => package.BreakfastIncluded,
            MealType.Lunch => package.LunchIncluded,
            MealType.Dinner => package.DinnerIncluded,
            _ => false
        };
    }

    private static CanteenDto MapCanteen(CanteenEntity x)
    {
        return new CanteenDto
        {
            CanteenId = x.CanteenId,
            HostelId = x.HostelId,
            CanteenName = x.CanteenName,
            Description = x.Description,
            IsActive = x.IsActive
        };
    }

    private static CanteenMenuItemDto MapMenuItem(CanteenMenuItem x)
    {
        return new CanteenMenuItemDto
        {
            MenuItemId = x.MenuItemId,
            CanteenId = x.CanteenId,
            ItemName = x.ItemName,
            Description = x.Description,
            MealType = x.MealType,
            Price = x.Price,
            IsAvailable = x.IsAvailable
        };
    }

    private static MealPackageResponseDto MapPackage(MealPackage x)
    {
        return new MealPackageResponseDto
        {
            MealPackageId = x.MealPackageId,
            CanteenId = x.CanteenId,
            CanteenName = x.Canteen?.CanteenName ?? string.Empty,
            PackageCode = x.PackageCode,
            PackageName = x.PackageName,
            PlanType = x.PlanType,
            BillingPeriod = x.BillingPeriod,
            BreakfastIncluded = x.BreakfastIncluded,
            LunchIncluded = x.LunchIncluded,
            DinnerIncluded = x.DinnerIncluded,
            Price = x.MonthlyPrice,
            IsActive = x.IsActive
        };
    }

    private static MealSubscriptionResponseDto MapSubscription(
        MealSubscription subscription,
        MealPackage package)
    {
        return new MealSubscriptionResponseDto
        {
            MealSubscriptionId = subscription.MealSubscriptionId,
            StudentId = subscription.StudentId,
            MealPackageId = subscription.MealPackageId,
            CanteenId = package.CanteenId,
            CanteenName = package.Canteen?.CanteenName ?? string.Empty,
            PackageCode = package.PackageCode,
            PackageName = package.PackageName,
            PlanType = package.PlanType,
            BillingPeriod = package.BillingPeriod,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            Amount = subscription.Amount,
            StudentFeeId = subscription.StudentFeeId,
            Status = subscription.Status,
            PaymentStatus = subscription.PaymentStatus,
            PaymentReference = subscription.PaymentReference,
            PaidAt = subscription.PaidAt,
            UnusedDays = 0,
            CarryForwardDays = 0
        };
    }

    private static MealUsageResponseDto MapUsage(MealUsage x)
    {
        return new MealUsageResponseDto
        {
            MealUsageId = x.MealUsageId,
            StudentId = x.StudentId,
            MealDate = x.MealDate,
            MealType = x.MealType,
            Status = x.Status,
            CollectedAt = x.CollectedAt
        };
    }
}
