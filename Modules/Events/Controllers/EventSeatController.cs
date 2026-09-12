using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Events.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventSeatController : ControllerBase
{
    private readonly IEventSeatService _eventSeatService;

    public EventSeatController(IEventSeatService eventSeatService)
    {
        _eventSeatService = eventSeatService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<EventSeatDto>>> GetAll()
    {
        return Ok(await _eventSeatService.GetAllAsync());
    }

    [HttpGet("event/{eventId:int}")]
    public async Task<ActionResult<List<EventSeatDto>>> GetByEventId(int eventId)
    {
        try
        {
            return Ok(await _eventSeatService.GetByEventIdAsync(eventId));
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{eventSeatId:int}")]
    public async Task<ActionResult<EventSeatDto>> GetById(int eventSeatId)
    {
        var seat = await _eventSeatService.GetByIdAsync(eventSeatId);
        return seat == null ? NotFound("Event seat not found.") : Ok(seat);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EventSeatDto>> Create([FromBody] EventSeatDto dto)
    {
        try
        {
            return Ok(await _eventSeatService.CreateAsync(dto));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{eventSeatId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int eventSeatId, [FromBody] EventSeatDto dto)
    {
        try
        {
            var updated = await _eventSeatService.UpdateAsync(eventSeatId, dto);
            return updated ? NoContent() : NotFound("Event seat not found.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
