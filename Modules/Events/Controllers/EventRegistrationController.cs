using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Services;
using CampusServicePortal_TicUnicorns.Modules.Events.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Events.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventRegistrationController : ControllerBase
{
    private readonly IEventRegistrationService _registrationService;

    public EventRegistrationController(
        IEventRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EventRegistrationDto>>> GetAll()
    {
        var registrations = await _registrationService.GetAllAsync();

        return Ok(registrations);
    }

    [HttpGet("{eventRegistrationId:int}")]
    public async Task<ActionResult<EventRegistrationDto>> GetById(
        int eventRegistrationId)
    {
        var registration =
            await _registrationService.GetByIdAsync(
                eventRegistrationId);

        if (registration == null)
            return NotFound("Event registration not found.");

        return Ok(registration);
    }

    [HttpGet("event/{eventId:int}")]
    public async Task<ActionResult<List<EventRegistrationDto>>> GetByEventId(
        int eventId)
    {
        try
        {
            var registrations =
                await _registrationService.GetByEventIdAsync(eventId);

            return Ok(registrations);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("student/{studentId:int}")]
    public async Task<ActionResult<List<EventRegistrationDto>>> GetByStudentId(
        int studentId)
    {
        var registrations =
            await _registrationService.GetByStudentIdAsync(studentId);

        return Ok(registrations);
    }

    [HttpPost]
    public async Task<ActionResult<EventRegistrationDto>> Register(
        [FromBody] EventRegistrationDto dto)
    {
        try
        {
            var registration =
                await _registrationService.RegisterAsync(dto);

            return Ok(registration);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{eventRegistrationId:int}")]
    public async Task<IActionResult> Update(
        int eventRegistrationId,
        [FromBody] EventRegistrationDto dto)
    {
        try
        {
            var updated =
                await _registrationService.UpdateAsync(
                    eventRegistrationId,
                    dto);

            if (!updated)
                return NotFound("Event registration not found.");

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}