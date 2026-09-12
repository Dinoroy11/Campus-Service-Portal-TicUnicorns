using System.Security.Claims;
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

    public CoachMeetingController(ICoachMeetingService coachMeetingService)
    {
        _coachMeetingService = coachMeetingService;
    }

    // ADMIN
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _coachMeetingService.GetAllAsync());
    }

    [HttpGet("{coachMeetingId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int coachMeetingId)
    {
        try
        {
            var meeting = await _coachMeetingService.GetByIdAsync(coachMeetingId);
            return meeting == null
                ? NotFound(new { message = "Coach meeting not found." })
                : Ok(meeting);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("event/{sportsEventId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetBySportsEventId(int sportsEventId)
    {
        try
        {
            return Ok(await _coachMeetingService
                .GetBySportsEventIdAsync(sportsEventId));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCoachMeetingDto dto)
    {
        try
        {
            var meeting = await _coachMeetingService.CreateAsync(
                GetCurrentUserId(),
                dto);

            return Ok(meeting);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{coachMeetingId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int coachMeetingId,
        [FromBody] UpdateCoachMeetingDto dto)
    {
        try
        {
            var meeting = await _coachMeetingService.UpdateAsync(
                coachMeetingId,
                GetCurrentUserId(),
                dto);

            return meeting == null
                ? NotFound(new { message = "Coach meeting not found." })
                : Ok(meeting);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // STUDENT: only students actively registered for the event can view meetings.
    [HttpGet("my/event/{sportsEventId:int}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyEventMeetings(int sportsEventId)
    {
        try
        {
            return Ok(await _coachMeetingService.GetForStudentEventAsync(
                GetCurrentUserId(),
                sportsEventId));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdText, out var userId))
            throw new UnauthorizedAccessException("Invalid user identity.");

        return userId;
    }
}
