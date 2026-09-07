using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Sports.Enums;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;

public interface ISportsRegistrationService
{
    Task<IEnumerable<SportsRegistrationDto>> GetAllAsync();

    Task<SportsRegistrationDto?> GetByIdAsync(
        int sportsRegistrationId);

    Task<IEnumerable<SportsRegistrationDto>>
        GetBySportsEventIdAsync(int sportsEventId);

    Task<IEnumerable<SportsRegistrationDto>>
        GetByStudentIdAsync(int studentId);

    Task<SportsRegistrationDto> CreateAsync(
        CreateSportsRegistrationDto dto);

    Task<SportsRegistrationDto?> UpdateStatusAsync(
        int sportsRegistrationId,
        SportsRegistrationStatus status);
}