using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Services;

public class CoachMeetingService : ICoachMeetingService
{
    private readonly ICoachMeetingRepository _coachMeetingRepository;
    private readonly ISportsEventRepository _sportsEventRepository;

    public CoachMeetingService(
        ICoachMeetingRepository coachMeetingRepository,
        ISportsEventRepository sportsEventRepository)
    {
        _coachMeetingRepository = coachMeetingRepository;
        _sportsEventRepository = sportsEventRepository;
    }

    public async Task<IEnumerable<CoachMeetingDto>> GetAllAsync()
    {
        var meetings = await _coachMeetingRepository.GetAllAsync();

        return meetings.Select(MapToDto);
    }

    public async Task<CoachMeetingDto?> GetByIdAsync(
        int coachMeetingId)
    {
        if (coachMeetingId <= 0)
            throw new ArgumentException("Invalid coach meeting ID.");

        var meeting =
            await _coachMeetingRepository.GetByIdAsync(
                coachMeetingId);

        return meeting == null
            ? null
            : MapToDto(meeting);
    }

    public async Task<IEnumerable<CoachMeetingDto>>
        GetBySportsEventIdAsync(int sportsEventId)
    {
        if (sportsEventId <= 0)
            throw new ArgumentException("Invalid sports event ID.");

        var meetings =
            await _coachMeetingRepository
                .GetBySportsEventIdAsync(sportsEventId);

        return meetings.Select(MapToDto);
    }

    public async Task<CoachMeetingDto> CreateAsync(
        CoachMeetingDto dto)
    {
        Validate(dto);

        var sportsEvent =
            await _sportsEventRepository
                .GetByIdAsync(dto.SportsEventId);

        if (sportsEvent == null)
            throw new ArgumentException(
                "Sports event does not exist.");

        var meeting = new CoachMeeting
        {
            SportsEventId = dto.SportsEventId,
            CreatedByUserId = dto.CreatedByUserId,
            MeetingDate = dto.MeetingDate,
            MeetingTime = dto.MeetingTime,
            Location = dto.Location,
            Notes = dto.Notes
        };

        await _coachMeetingRepository.AddAsync(meeting);

        return MapToDto(meeting);
    }

    public async Task<CoachMeetingDto?> UpdateAsync(
        int coachMeetingId,
        CoachMeetingDto dto)
    {
        if (coachMeetingId <= 0)
            throw new ArgumentException(
                "Invalid coach meeting ID.");

        Validate(dto);

        var meeting =
            await _coachMeetingRepository
                .GetByIdAsync(coachMeetingId);

        if (meeting == null)
            return null;

        var sportsEvent =
            await _sportsEventRepository
                .GetByIdAsync(dto.SportsEventId);

        if (sportsEvent == null)
            throw new ArgumentException(
                "Sports event does not exist.");

        meeting.SportsEventId = dto.SportsEventId;
        meeting.CreatedByUserId = dto.CreatedByUserId;
        meeting.MeetingDate = dto.MeetingDate;
        meeting.MeetingTime = dto.MeetingTime;
        meeting.Location = dto.Location;
        meeting.Notes = dto.Notes;

        await _coachMeetingRepository.UpdateAsync(meeting);

        return MapToDto(meeting);
    }

    private static void Validate(CoachMeetingDto dto)
    {
        if (dto.SportsEventId <= 0)
            throw new ArgumentException(
                "Invalid sports event ID.");

        if (dto.MeetingDate == default)
            throw new ArgumentException(
                "Meeting date is required.");

        if (string.IsNullOrWhiteSpace(dto.Location))
            throw new ArgumentException(
                "Meeting location is required.");
    }

    private static CoachMeetingDto MapToDto(
        CoachMeeting meeting)
    {
        return new CoachMeetingDto
        {
            CoachMeetingId = meeting.CoachMeetingId,
            SportsEventId = meeting.SportsEventId,
            CreatedByUserId = meeting.CreatedByUserId,
            MeetingDate = meeting.MeetingDate,
            MeetingTime = meeting.MeetingTime,
            Location = meeting.Location,
            Notes = meeting.Notes
        };
    }
}