using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Services;

public class SportsEventDepartmentLimitService
    : ISportsEventDepartmentLimitService
{
    private readonly ISportsEventDepartmentLimitRepository
        _departmentLimitRepository;

    private readonly ISportsEventRepository
        _sportsEventRepository;

    public SportsEventDepartmentLimitService(
        ISportsEventDepartmentLimitRepository departmentLimitRepository,
        ISportsEventRepository sportsEventRepository)
    {
        _departmentLimitRepository = departmentLimitRepository;
        _sportsEventRepository = sportsEventRepository;
    }

    public async Task<IEnumerable<SportsEventDepartmentLimitDto>>
        GetBySportsEventIdAsync(int sportsEventId)
    {
        if (sportsEventId <= 0)
            throw new ArgumentException("Invalid sports event ID.");

        var limits =
            await _departmentLimitRepository
                .GetBySportsEventIdAsync(sportsEventId);

        return limits.Select(MapToDto);
    }

    public async Task<SportsEventDepartmentLimitDto>
        CreateAsync(SportsEventDepartmentLimitDto dto)
    {
        if (dto.SportsEventId <= 0)
            throw new ArgumentException("Invalid sports event ID.");

        if (dto.DepartmentId <= 0)
            throw new ArgumentException("Invalid department ID.");

        if (dto.RegistrationLimit <= 0)
            throw new ArgumentException(
                "Registration limit must be greater than zero.");

        var sportsEvent =
            await _sportsEventRepository
                .GetByIdAsync(dto.SportsEventId);

        if (sportsEvent == null)
            throw new ArgumentException(
                "Sports event does not exist.");

        var alreadyExists =
            await _departmentLimitRepository
                .ExistsByEventAndDepartmentAsync(
                    dto.SportsEventId,
                    dto.DepartmentId);

        if (alreadyExists)
            throw new InvalidOperationException(
                "A department limit already exists for this sports event.");

        var departmentLimit = new SportsEventDepartmentLimit
        {
            SportsEventId = dto.SportsEventId,
            DepartmentId = dto.DepartmentId,
            RegistrationLimit = dto.RegistrationLimit
        };

        await _departmentLimitRepository.AddAsync(departmentLimit);

        return MapToDto(departmentLimit);
    }

    public async Task<SportsEventDepartmentLimitDto?>
        UpdateAsync(
            int sportsEventDepartmentLimitId,
            SportsEventDepartmentLimitDto dto)
    {
        if (sportsEventDepartmentLimitId <= 0)
            throw new ArgumentException(
                "Invalid department limit ID.");

        if (dto.SportsEventId <= 0)
            throw new ArgumentException("Invalid sports event ID.");

        if (dto.DepartmentId <= 0)
            throw new ArgumentException("Invalid department ID.");

        if (dto.RegistrationLimit <= 0)
            throw new ArgumentException(
                "Registration limit must be greater than zero.");

        var departmentLimit =
            await _departmentLimitRepository
                .GetByIdAsync(sportsEventDepartmentLimitId);

        if (departmentLimit == null)
            return null;

        var sportsEvent =
            await _sportsEventRepository
                .GetByIdAsync(dto.SportsEventId);

        if (sportsEvent == null)
            throw new ArgumentException(
                "Sports event does not exist.");

        var duplicate =
            await _departmentLimitRepository
                .ExistsByEventAndDepartmentAsync(
                    dto.SportsEventId,
                    dto.DepartmentId);

        if (duplicate &&
            (departmentLimit.SportsEventId != dto.SportsEventId ||
             departmentLimit.DepartmentId != dto.DepartmentId))
        {
            throw new InvalidOperationException(
                "A department limit already exists for this sports event.");
        }

        departmentLimit.SportsEventId = dto.SportsEventId;
        departmentLimit.DepartmentId = dto.DepartmentId;
        departmentLimit.RegistrationLimit = dto.RegistrationLimit;

        await _departmentLimitRepository.UpdateAsync(departmentLimit);

        return MapToDto(departmentLimit);
    }

    private static SportsEventDepartmentLimitDto MapToDto(
        SportsEventDepartmentLimit departmentLimit)
    {
        return new SportsEventDepartmentLimitDto
        {
            SportsEventDepartmentLimitId =
                departmentLimit.SportsEventDepartmentLimitId,

            SportsEventId =
                departmentLimit.SportsEventId,

            DepartmentId =
                departmentLimit.DepartmentId,

            RegistrationLimit =
                departmentLimit.RegistrationLimit
        };
    }
}