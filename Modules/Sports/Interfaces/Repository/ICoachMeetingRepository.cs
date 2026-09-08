using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;

public interface ICoachMeetingRepository
{
    Task<CoachMeeting?> GetByIdAsync(int coachMeetingId);

    Task<IEnumerable<CoachMeeting>> GetAllAsync();

    Task<IEnumerable<CoachMeeting>> GetBySportsEventIdAsync(
        int sportsEventId);

    Task AddAsync(CoachMeeting coachMeeting);

    Task UpdateAsync(CoachMeeting coachMeeting);

    Task<bool> ExistsAsync(int coachMeetingId);
}