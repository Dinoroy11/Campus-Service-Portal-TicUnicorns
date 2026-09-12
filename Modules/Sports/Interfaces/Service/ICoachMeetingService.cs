using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;

public interface ICoachMeetingService
{
    Task<IEnumerable<CoachMeetingDto>> GetAllAsync();

    Task<CoachMeetingDto?> GetByIdAsync(int coachMeetingId);

    Task<IEnumerable<CoachMeetingDto>> GetBySportsEventIdAsync(
        int sportsEventId);

    Task<IEnumerable<CoachMeetingDto>> GetForStudentEventAsync(
        int userId,
        int sportsEventId);

    Task<CoachMeetingDto> CreateAsync(
        int createdByUserId,
        CreateCoachMeetingDto dto);

    Task<CoachMeetingDto?> UpdateAsync(
        int coachMeetingId,
        int updatedByUserId,
        UpdateCoachMeetingDto dto);
}
