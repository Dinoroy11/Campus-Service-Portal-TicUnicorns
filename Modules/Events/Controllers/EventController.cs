using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Events.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        return Ok(await _eventService.GetAllAsync());
    }

    [HttpGet("{eventId:int}")]
    public async Task<ActionResult<EventDto>> GetById(int eventId)
    {
        var eventDto = await _eventService.GetByIdAsync(eventId);
        return eventDto == null ? NotFound("Event not found.") : Ok(eventDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EventDto>> Create([FromBody] CreateEventDto dto)
    {
        try
        {
            return Ok(await _eventService.CreateAsync(dto));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{eventId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int eventId, [FromBody] UpdateEventDto dto)
    {
        try
        {
            var updated = await _eventService.UpdateAsync(eventId, dto);
            return updated ? NoContent() : NotFound("Event not found.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
