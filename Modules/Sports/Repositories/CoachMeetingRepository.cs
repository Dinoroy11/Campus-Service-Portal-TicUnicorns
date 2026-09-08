using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Repositories;

public class CoachMeetingRepository : ICoachMeetingRepository
{
    public Task<CoachMeeting?> GetByIdAsync(int coachMeetingId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CoachMeeting>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CoachMeeting>> GetBySportsEventIdAsync(
        int sportsEventId)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(CoachMeeting coachMeeting)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(CoachMeeting coachMeeting)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(int coachMeetingId)
    {
        throw new NotImplementedException();
    }
}