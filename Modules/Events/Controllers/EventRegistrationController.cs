using System.Security.Claims;
using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Events.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Events.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventRegistrationController : ControllerBase
{
    private readonly IEventRegistrationService _registrationService;
    private readonly IStudentRepository _studentRepository;

    public EventRegistrationController(
        IEventRegistrationService registrationService,
        IStudentRepository studentRepository)
    {
        _registrationService = registrationService;
        _studentRepository = studentRepository;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<EventRegistrationDto>>> GetAll()
    {
        return Ok(await _registrationService.GetAllAsync());
    }

    [HttpGet("{eventRegistrationId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EventRegistrationDto>> GetById(int eventRegistrationId)
    {
        var registration = await _registrationService.GetByIdAsync(eventRegistrationId);
        return registration == null
            ? NotFound("Event registration not found.")
            : Ok(registration);
    }

    [HttpGet("event/{eventId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<EventRegistrationDto>>> GetByEventId(int eventId)
    {
        try
        {
            return Ok(await _registrationService.GetByEventIdAsync(eventId));
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<List<EventRegistrationDto>>> GetMy()
    {
        var student = await GetCurrentStudentAsync();
        return Ok(await _registrationService.GetByStudentIdAsync(student.StudentId));
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<EventRegistrationDto>> Register([FromBody] EventRegistrationDto dto)
    {
        try
        {
            var student = await GetCurrentStudentAsync();
            dto.StudentId = student.StudentId;
            return Ok(await _registrationService.RegisterAsync(dto));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{eventRegistrationId:int}/confirm")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<EventRegistrationDto>> ConfirmFree(int eventRegistrationId)
    {
        try
        {
            var student = await GetCurrentStudentAsync();
            return Ok(await _registrationService.ConfirmFreeRegistrationAsync(
                eventRegistrationId,
                student.StudentId));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{eventRegistrationId:int}")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<EventRegistrationDto>> Cancel(int eventRegistrationId)
    {
        try
        {
            var student = await GetCurrentStudentAsync();
            return Ok(await _registrationService.CancelAsync(
                eventRegistrationId,
                student.StudentId));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{eventRegistrationId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int eventRegistrationId,
        [FromBody] EventRegistrationDto dto)
    {
        try
        {
            var updated = await _registrationService.UpdateAsync(eventRegistrationId, dto);
            return updated ? NoContent() : NotFound("Event registration not found.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private async Task<CampusServicePortal_TicUnicorns.Modules.Students.Entities.Student>
        GetCurrentStudentAsync()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdText, out var userId))
            throw new UnauthorizedAccessException("Invalid user identity.");

        return await _studentRepository.GetByUserIdAsync(userId)
            ?? throw new UnauthorizedAccessException("Student profile was not found for the logged-in user.");
    }
}
