using CampusServicePortal.Modules.Notifications.DTOs;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Enums;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Services;

public class CoachMeetingService : ICoachMeetingService
{
    private readonly ICoachMeetingRepository _coachMeetingRepository;
    private readonly ISportsEventRepository _sportsEventRepository;
    private readonly ISportsRegistrationRepository _registrationRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly INotificationService _notificationService;

    public CoachMeetingService(
        ICoachMeetingRepository coachMeetingRepository,
        ISportsEventRepository sportsEventRepository,
        ISportsRegistrationRepository registrationRepository,
        IStudentRepository studentRepository,
        INotificationService notificationService)
    {
        _coachMeetingRepository = coachMeetingRepository;
        _sportsEventRepository = sportsEventRepository;
        _registrationRepository = registrationRepository;
        _studentRepository = studentRepository;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<CoachMeetingDto>> GetAllAsync()
    {
        var meetings = await _coachMeetingRepository.GetAllAsync();
        return meetings.Select(MapToDto);
    }

    public async Task<CoachMeetingDto?> GetByIdAsync(int coachMeetingId)
    {
        if (coachMeetingId <= 0)
            throw new ArgumentException("Invalid coach meeting ID.");

        var meeting = await _coachMeetingRepository.GetByIdAsync(coachMeetingId);
        return meeting == null ? null : MapToDto(meeting);
    }

    public async Task<IEnumerable<CoachMeetingDto>> GetBySportsEventIdAsync(
        int sportsEventId)
    {
        if (sportsEventId <= 0)
            throw new ArgumentException("Invalid sports event ID.");

        var meetings =
            await _coachMeetingRepository.GetBySportsEventIdAsync(sportsEventId);

        return meetings.Select(MapToDto);
    }

    public async Task<IEnumerable<CoachMeetingDto>> GetForStudentEventAsync(
        int userId,
        int sportsEventId)
    {
        if (sportsEventId <= 0)
            throw new ArgumentException("Invalid sports event ID.");

        var student = await _studentRepository.GetByUserIdAsync(userId);

        if (student == null || !student.IsActive)
            throw new UnauthorizedAccessException("An active student profile is required.");

        var registration =
            await _registrationRepository.GetByEventAndStudentAsync(
                sportsEventId,
                student.StudentId);

        if (registration == null ||
            registration.Status == SportsRegistrationStatus.Cancelled)
        {
            throw new UnauthorizedAccessException(
                "You must have an active registration for this sports event to view coach meetings.");
        }

        var meetings =
            await _coachMeetingRepository.GetBySportsEventIdAsync(sportsEventId);

        return meetings.Select(MapToDto);
    }

    public async Task<CoachMeetingDto> CreateAsync(
        int createdByUserId,
        CreateCoachMeetingDto dto)
    {
        Validate(dto.SportsEventId, dto.MeetingDate, dto.Location);

        var sportsEvent =
            await _sportsEventRepository.GetByIdAsync(dto.SportsEventId);

        if (sportsEvent == null)
            throw new ArgumentException("Sports event does not exist.");

        var meeting = new CoachMeeting
        {
            SportsEventId = dto.SportsEventId,
            CreatedByUserId = createdByUserId,
            MeetingDate = dto.MeetingDate,
            MeetingTime = dto.MeetingTime,
            Location = dto.Location?.Trim(),
            Notes = dto.Notes?.Trim()
        };

        await _coachMeetingRepository.AddAsync(meeting);
        await NotifyRegisteredStudentsAsync(meeting, sportsEvent.EventName, "scheduled");

        return MapToDto(meeting);
    }

    public async Task<CoachMeetingDto?> UpdateAsync(
        int coachMeetingId,
        int updatedByUserId,
        UpdateCoachMeetingDto dto)
    {
        if (coachMeetingId <= 0)
            throw new ArgumentException("Invalid coach meeting ID.");

        Validate(dto.SportsEventId, dto.MeetingDate, dto.Location);

        var meeting = await _coachMeetingRepository.GetByIdAsync(coachMeetingId);
        if (meeting == null)
            return null;

        var sportsEvent =
            await _sportsEventRepository.GetByIdAsync(dto.SportsEventId);

        if (sportsEvent == null)
            throw new ArgumentException("Sports event does not exist.");

        meeting.SportsEventId = dto.SportsEventId;
        meeting.CreatedByUserId = updatedByUserId;
        meeting.MeetingDate = dto.MeetingDate;
        meeting.MeetingTime = dto.MeetingTime;
        meeting.Location = dto.Location?.Trim();
        meeting.Notes = dto.Notes?.Trim();

        await _coachMeetingRepository.UpdateAsync(meeting);
        await NotifyRegisteredStudentsAsync(meeting, sportsEvent.EventName, "updated");

        return MapToDto(meeting);
    }

    private async Task NotifyRegisteredStudentsAsync(
        CoachMeeting meeting,
        string eventName,
        string action)
    {
        var registrations =
            await _registrationRepository.GetBySportsEventIdAsync(
                meeting.SportsEventId);

        foreach (var registration in registrations
                     .Where(x => x.Status != SportsRegistrationStatus.Cancelled))
        {
            var student = await _studentRepository.GetByIdAsync(registration.StudentId);

            if (student?.UserId is not int userId || userId <= 0)
                continue;

            await _notificationService.CreateAsync(new NotificationCreateDto
            {
                UserId = userId,
                Title = "Sports Coach Meeting",
                Message =
                    $"A coach meeting for '{eventName}' was {action}. " +
                    $"Date: {meeting.MeetingDate:yyyy-MM-dd}, Time: {meeting.MeetingTime}, " +
                    $"Location: {meeting.Location}.",
                ReferenceType = "CoachMeeting",
                ReferenceId = meeting.CoachMeetingId
            });
        }
    }

    private static void Validate(
        int sportsEventId,
        DateTime meetingDate,
        string? location)
    {
        if (sportsEventId <= 0)
            throw new ArgumentException("Invalid sports event ID.");

        if (meetingDate == default)
            throw new ArgumentException("Meeting date is required.");

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Meeting location is required.");
    }

    private static CoachMeetingDto MapToDto(CoachMeeting meeting)
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
