using CampusServicePortal.Modules.Notifications.DTOs;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Enums;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Services;

public class SportsRegistrationService : ISportsRegistrationService
{
    private readonly ISportsRegistrationRepository _registrationRepository;
    private readonly ISportsEventRepository _sportsEventRepository;
    private readonly ISportsEventDepartmentLimitRepository _departmentLimitRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IStudentService _studentService;
    private readonly INotificationService _notificationService;

    public SportsRegistrationService(
        ISportsRegistrationRepository registrationRepository,
        ISportsEventRepository sportsEventRepository,
        ISportsEventDepartmentLimitRepository departmentLimitRepository,
        IStudentRepository studentRepository,
        IStudentService studentService,
        INotificationService notificationService)
    {
        _registrationRepository = registrationRepository;
        _sportsEventRepository = sportsEventRepository;
        _departmentLimitRepository = departmentLimitRepository;
        _studentRepository = studentRepository;
        _studentService = studentService;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<SportsRegistrationDto>> GetAllAsync()
    {
        var registrations = await _registrationRepository.GetAllAsync();
        return registrations.Select(MapToDto);
    }

    public async Task<SportsRegistrationDto?> GetByIdAsync(int sportsRegistrationId)
    {
        if (sportsRegistrationId <= 0)
            throw new ArgumentException("Invalid sports registration ID.");

        var registration = await _registrationRepository.GetByIdAsync(sportsRegistrationId);
        return registration == null ? null : MapToDto(registration);
    }

    public async Task<IEnumerable<SportsRegistrationDto>> GetBySportsEventIdAsync(
        int sportsEventId)
    {
        if (sportsEventId <= 0)
            throw new ArgumentException("Invalid sports event ID.");

        var registrations =
            await _registrationRepository.GetBySportsEventIdAsync(sportsEventId);

        return registrations.Select(MapToDto);
    }

    public async Task<IEnumerable<SportsRegistrationDto>> GetMyAsync(int userId)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var registrations =
            await _registrationRepository.GetByStudentIdAsync(student.StudentId);

        return registrations.Select(MapToDto);
    }

    public async Task<SportsRegistrationDto> CreateMyAsync(
        int userId,
        CreateSportsRegistrationDto dto)
    {
        if (dto.SportsEventId <= 0)
            throw new ArgumentException("Invalid sports event ID.");

        var student = await GetStudentByUserIdAsync(userId);
        var sportsEvent =
            await _sportsEventRepository.GetByIdAsync(dto.SportsEventId);

        if (sportsEvent == null)
            throw new ArgumentException("Sports event does not exist.");

        if (!sportsEvent.IsActive)
            throw new InvalidOperationException("Sports event is not active.");

        EnsureEventNotFinished(sportsEvent);

        var existing = await _registrationRepository.GetByEventAndStudentAsync(
            dto.SportsEventId,
            student.StudentId);

        if (existing != null && existing.Status != SportsRegistrationStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "You are already registered for this sports event.");
        }

        await EnsureDepartmentCapacityAsync(dto.SportsEventId, student.StudentId);

        SportsRegistration registration;

        if (existing != null)
        {
            existing.Status = SportsRegistrationStatus.Registered;
            existing.RegisteredAt = DateTime.UtcNow;
            await _registrationRepository.UpdateAsync(existing);
            registration = existing;
        }
        else
        {
            registration = new SportsRegistration
            {
                SportsEventId = dto.SportsEventId,
                StudentId = student.StudentId,
                Status = SportsRegistrationStatus.Registered,
                RegisteredAt = DateTime.UtcNow
            };

            await _registrationRepository.AddAsync(registration);
        }

        await NotifyStudentAsync(
            student.UserId,
            "Sports Registration Submitted",
            $"Your registration for '{sportsEvent.EventName}' has been submitted.",
            registration.SportsRegistrationId);

        return MapToDto(registration);
    }

    public async Task<SportsRegistrationDto?> UpdateStatusAsync(
        int sportsRegistrationId,
        SportsRegistrationStatus status)
    {
        if (sportsRegistrationId <= 0)
            throw new ArgumentException("Invalid sports registration ID.");

        if (!Enum.IsDefined(status))
            throw new ArgumentException("Invalid registration status.");

        if (status == SportsRegistrationStatus.Registered)
            throw new InvalidOperationException(
                "Admin can only confirm or cancel an existing registration.");

        var registration =
            await _registrationRepository.GetByIdAsync(sportsRegistrationId);

        if (registration == null)
            return null;

        if (!IsValidAdminStatusTransition(registration.Status, status))
        {
            throw new InvalidOperationException(
                $"Invalid status transition from {registration.Status} to {status}.");
        }

        registration.Status = status;
        await _registrationRepository.UpdateAsync(registration);

        var student = await _studentRepository.GetByIdAsync(registration.StudentId);
        var sportsEvent =
            await _sportsEventRepository.GetByIdAsync(registration.SportsEventId);

        if (student?.UserId is int studentUserId)
        {
            var eventName = sportsEvent?.EventName ?? "sports event";
            await NotifyStudentAsync(
                studentUserId,
                "Sports Registration Updated",
                $"Your registration for '{eventName}' is now {status}.",
                registration.SportsRegistrationId);
        }

        return MapToDto(registration);
    }

    public async Task<SportsRegistrationDto?> CancelMyAsync(
        int userId,
        int sportsRegistrationId)
    {
        if (sportsRegistrationId <= 0)
            throw new ArgumentException("Invalid sports registration ID.");

        var student = await GetStudentByUserIdAsync(userId);
        var registration =
            await _registrationRepository.GetByIdAsync(sportsRegistrationId);

        if (registration == null)
            return null;

        if (registration.StudentId != student.StudentId)
            throw new UnauthorizedAccessException(
                "You can only cancel your own sports registration.");

        if (registration.Status == SportsRegistrationStatus.Cancelled)
            throw new InvalidOperationException("Registration is already cancelled.");

        var sportsEvent =
            await _sportsEventRepository.GetByIdAsync(registration.SportsEventId);

        if (sportsEvent != null)
            EnsureEventNotFinished(sportsEvent);

        registration.Status = SportsRegistrationStatus.Cancelled;
        await _registrationRepository.UpdateAsync(registration);

        await NotifyStudentAsync(
            student.UserId,
            "Sports Registration Cancelled",
            $"Your registration for '{sportsEvent?.EventName ?? "sports event"}' has been cancelled.",
            registration.SportsRegistrationId);

        return MapToDto(registration);
    }

    private async Task EnsureDepartmentCapacityAsync(
        int sportsEventId,
        int studentId)
    {
        var departmentId = await _studentService.GetDepartmentIdAsync(studentId);

        if (departmentId is null)
            throw new InvalidOperationException(
                "Student department could not be determined.");

        var departmentLimits =
            await _departmentLimitRepository.GetBySportsEventIdAsync(sportsEventId);

        var departmentLimit = departmentLimits.FirstOrDefault(
            x => x.DepartmentId == departmentId.Value);

        if (departmentLimit is null)
            throw new InvalidOperationException(
                "No registration limit is configured for your department.");

        var currentRegistrationCount =
            await _registrationRepository.CountByEventAndStudentDepartmentAsync(
                sportsEventId,
                departmentId.Value);

        if (currentRegistrationCount >= departmentLimit.RegistrationLimit)
        {
            throw new InvalidOperationException(
                "Registration limit for your department has been reached.");
        }
    }

    private async Task<Student> GetStudentByUserIdAsync(
        int userId)
    {
        if (userId <= 0)
            throw new UnauthorizedAccessException("Invalid user identity.");

        var student = await _studentRepository.GetByUserIdAsync(userId);

        if (student == null || !student.IsActive)
            throw new UnauthorizedAccessException(
                "An active student profile is required.");

        return student;
    }

    private static void EnsureEventNotFinished(SportsEvent sportsEvent)
    {
        var now = DateTime.UtcNow;

        if (sportsEvent.EventDate.Date < now.Date ||
            (sportsEvent.EventDate.Date == now.Date && sportsEvent.EndTime <= now.TimeOfDay))
        {
            throw new InvalidOperationException(
                "Registration changes are not allowed after the sports event has ended.");
        }
    }

    private static bool IsValidAdminStatusTransition(
        SportsRegistrationStatus currentStatus,
        SportsRegistrationStatus newStatus)
    {
        return currentStatus switch
        {
            SportsRegistrationStatus.Registered =>
                newStatus == SportsRegistrationStatus.Confirmed ||
                newStatus == SportsRegistrationStatus.Cancelled,

            SportsRegistrationStatus.Confirmed =>
                newStatus == SportsRegistrationStatus.Cancelled,

            SportsRegistrationStatus.Cancelled => false,
            _ => false
        };
    }

    private async Task NotifyStudentAsync(
        int? userId,
        string title,
        string message,
        int referenceId)
    {
        if (userId is null or <= 0)
            return;

        await _notificationService.CreateAsync(new NotificationCreateDto
        {
            UserId = userId.Value,
            Title = title,
            Message = message,
            ReferenceType = "SportsRegistration",
            ReferenceId = referenceId
        });
    }

    private static SportsRegistrationDto MapToDto(SportsRegistration registration)
    {
        return new SportsRegistrationDto
        {
            SportsRegistrationId = registration.SportsRegistrationId,
            SportsEventId = registration.SportsEventId,
            StudentId = registration.StudentId,
            Status = registration.Status,
            RegisteredAt = registration.RegisteredAt
        };
    }
}
