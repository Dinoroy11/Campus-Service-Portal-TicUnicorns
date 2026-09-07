using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoachMeetingController : ControllerBase
{
    private readonly ICoachMeetingService _coachMeetingService;

    public CoachMeetingController(
        ICoachMeetingService coachMeetingService)
    {
        _coachMeetingService = coachMeetingService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoachMeetingDto>>>
        GetAll()
    {
        var meetings =
            await _coachMeetingService.GetAllAsync();

        return Ok(meetings);
    }

    [HttpGet("{coachMeetingId:int}")]
    public async Task<ActionResult<CoachMeetingDto>> GetById(
        int coachMeetingId)
    {
        var meeting =
            await _coachMeetingService
                .GetByIdAsync(coachMeetingId);

        if (meeting == null)
            return NotFound();

        return Ok(meeting);
    }

    [HttpGet("event/{sportsEventId:int}")]
    public async Task<ActionResult<IEnumerable<CoachMeetingDto>>>
        GetBySportsEventId(int sportsEventId)
    {
        var meetings =
            await _coachMeetingService
                .GetBySportsEventIdAsync(sportsEventId);

        return Ok(meetings);
    }

    [HttpPost]
    public async Task<ActionResult<CoachMeetingDto>> Create(
        CoachMeetingDto dto)
    {
        var meeting =
            await _coachMeetingService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                coachMeetingId =
                    meeting.CoachMeetingId
            },
            meeting);
    }

    [HttpPut("{coachMeetingId:int}")]
    public async Task<ActionResult<CoachMeetingDto>> Update(
        int coachMeetingId,
        CoachMeetingDto dto)
    {
        var meeting =
            await _coachMeetingService.UpdateAsync(
                coachMeetingId,
                dto);

        if (meeting == null)
            return NotFound();

        return Ok(meeting);
    }
}