using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Enums;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Services;

public class SportsRegistrationService : ISportsRegistrationService
{
    private readonly ISportsRegistrationRepository _registrationRepository;
    private readonly ISportsEventRepository _sportsEventRepository;
    private readonly ISportsEventDepartmentLimitRepository _departmentLimitRepository;
    private readonly IStudentService _studentService;

    public SportsRegistrationService(
        ISportsRegistrationRepository registrationRepository,
        ISportsEventRepository sportsEventRepository,
        ISportsEventDepartmentLimitRepository departmentLimitRepository,
        IStudentService studentService)
    {
        _registrationRepository = registrationRepository;
        _sportsEventRepository = sportsEventRepository;
        _departmentLimitRepository = departmentLimitRepository;
        _studentService = studentService;
    }

    public async Task<IEnumerable<SportsRegistrationDto>> GetAllAsync()
    {
        var registrations = await _registrationRepository.GetAllAsync();

        return registrations.Select(MapToDto);
    }

    public async Task<SportsRegistrationDto?> GetByIdAsync(
        int sportsRegistrationId)
    {
        if (sportsRegistrationId <= 0)
            throw new ArgumentException("Invalid sports registration ID.");

        var registration =
            await _registrationRepository.GetByIdAsync(
                sportsRegistrationId);

        return registration == null
            ? null
            : MapToDto(registration);
    }

    public async Task<IEnumerable<SportsRegistrationDto>>
        GetBySportsEventIdAsync(int sportsEventId)
    {
        if (sportsEventId <= 0)
            throw new ArgumentException("Invalid sports event ID.");

        var registrations =
            await _registrationRepository
                .GetBySportsEventIdAsync(sportsEventId);

        return registrations.Select(MapToDto);
    }

    public async Task<IEnumerable<SportsRegistrationDto>>
        GetByStudentIdAsync(int studentId)
    {
        if (studentId <= 0)
            throw new ArgumentException("Invalid student ID.");

        var registrations =
            await _registrationRepository
                .GetByStudentIdAsync(studentId);

        return registrations.Select(MapToDto);
    }

    public async Task<SportsRegistrationDto> CreateAsync(
        CreateSportsRegistrationDto dto)
    {
        if (dto.SportsEventId <= 0)
            throw new ArgumentException("Invalid sports event ID.");

        if (dto.StudentId <= 0)
            throw new ArgumentException("Invalid student ID.");

        var sportsEvent =
            await _sportsEventRepository
                .GetByIdAsync(dto.SportsEventId);

        if (sportsEvent == null)
            throw new ArgumentException(
                "Sports event does not exist.");

        if (!sportsEvent.IsActive)
            throw new InvalidOperationException(
                "Sports event is not active.");

        var duplicate =
            await _registrationRepository
                .ExistsByEventAndStudentAsync(
                    dto.SportsEventId,
                    dto.StudentId);

        if (duplicate)
            throw new InvalidOperationException(
                "Student is already registered for this sports event.");

        // Get the student's department through the Students module.
        var departmentId =
            await _studentService.GetDepartmentIdAsync(dto.StudentId);

        if (departmentId is null)
            throw new InvalidOperationException(
                "Student department could not be determined.");

        // Get department-specific limits configured for this event.
        var departmentLimits =
            await _departmentLimitRepository
                .GetBySportsEventIdAsync(dto.SportsEventId);

        var departmentLimit =
            departmentLimits.FirstOrDefault(
                x => x.DepartmentId == departmentId.Value);

        if (departmentLimit is null)
            throw new InvalidOperationException(
                "No registration limit is configured for the student's department.");

        // Count current registrations for the student's department.
        var currentRegistrationCount =
            await _registrationRepository
                .CountByEventAndStudentDepartmentAsync(
                    dto.SportsEventId,
                    departmentId.Value);

        if (currentRegistrationCount >=
            departmentLimit.RegistrationLimit)
        {
            throw new InvalidOperationException(
                "Registration limit for the student's department has been reached.");
        }

        var registration = new SportsRegistration
        {
            SportsEventId = dto.SportsEventId,
            StudentId = dto.StudentId,
            Status = SportsRegistrationStatus.Registered,
            RegisteredAt = DateTime.UtcNow
        };

        await _registrationRepository.AddAsync(registration);

        return MapToDto(registration);
    }

    public async Task<SportsRegistrationDto?> UpdateStatusAsync(
        int sportsRegistrationId,
        SportsRegistrationStatus status)
    {
        if (sportsRegistrationId <= 0)
            throw new ArgumentException(
                "Invalid sports registration ID.");

        if (!Enum.IsDefined(status))
            throw new ArgumentException(
                "Invalid registration status.");

        var registration =
            await _registrationRepository
                .GetByIdAsync(sportsRegistrationId);

        if (registration == null)
            return null;

        if (!IsValidStatusTransition(
                registration.Status,
                status))
        {
            throw new InvalidOperationException(
                $"Invalid status transition from " +
                $"{registration.Status} to {status}.");
        }

        registration.Status = status;

        await _registrationRepository.UpdateAsync(registration);

        return MapToDto(registration);
    }

    private static bool IsValidStatusTransition(
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

            SportsRegistrationStatus.Cancelled =>
                false,

            _ => false
        };
    }

    private static SportsRegistrationDto MapToDto(
        SportsRegistration registration)
    {
        return new SportsRegistrationDto
        {
            SportsRegistrationId =
                registration.SportsRegistrationId,

            SportsEventId =
                registration.SportsEventId,

            StudentId =
                registration.StudentId,

            Status =
                registration.Status,

            RegisteredAt =
                registration.RegisteredAt
        };
    }
}