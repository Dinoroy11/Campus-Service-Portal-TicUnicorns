using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;

public interface ICoachMeetingService
{
    Task<IEnumerable<CoachMeetingDto>> GetAllAsync();

    Task<CoachMeetingDto?> GetByIdAsync(int coachMeetingId);

    Task<IEnumerable<CoachMeetingDto>>
        GetBySportsEventIdAsync(int sportsEventId);

    Task<CoachMeetingDto> CreateAsync(CoachMeetingDto dto);

    Task<CoachMeetingDto?> UpdateAsync(
        int coachMeetingId,
        CoachMeetingDto dto);
}