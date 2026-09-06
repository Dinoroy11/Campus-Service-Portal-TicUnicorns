using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Events.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EventDto>>> GetAll()
    {
        var events = await _eventService.GetAllAsync();

        return Ok(events);
    }

    [HttpGet("{eventId:int}")]
    public async Task<ActionResult<EventDto>> GetById(int eventId)
    {
        var eventDto = await _eventService.GetByIdAsync(eventId);

        if (eventDto == null)
            return NotFound("Event not found.");

        return Ok(eventDto);
    }

    [HttpPost]
    public async Task<ActionResult<EventDto>> Create(
        [FromBody] CreateEventDto dto)
    {
        try
        {
            var createdEvent = await _eventService.CreateAsync(dto);

            return Ok(createdEvent);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{eventId:int}")]
    public async Task<IActionResult> Update(
        int eventId,
        [FromBody] UpdateEventDto dto)
    {
        try
        {
            var updated = await _eventService.UpdateAsync(
                eventId,
                dto);

            if (!updated)
                return NotFound("Event not found.");

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}