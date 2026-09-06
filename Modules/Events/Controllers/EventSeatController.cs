using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Events.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventSeatController : ControllerBase
{
    private readonly IEventSeatService _eventSeatService;

    public EventSeatController(IEventSeatService eventSeatService)
    {
        _eventSeatService = eventSeatService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EventSeatDto>>> GetAll()
    {
        var seats = await _eventSeatService.GetAllAsync();

        return Ok(seats);
    }

    [HttpGet("event/{eventId:int}")]
    public async Task<ActionResult<List<EventSeatDto>>> GetByEventId(
        int eventId)
    {
        try
        {
            var seats = await _eventSeatService.GetByEventIdAsync(eventId);

            return Ok(seats);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{eventSeatId:int}")]
    public async Task<ActionResult<EventSeatDto>> GetById(
        int eventSeatId)
    {
        var seat = await _eventSeatService.GetByIdAsync(eventSeatId);

        if (seat == null)
            return NotFound("Event seat not found.");

        return Ok(seat);
    }

    [HttpPost]
    public async Task<ActionResult<EventSeatDto>> Create(
        [FromBody] EventSeatDto dto)
    {
        try
        {
            var createdSeat = await _eventSeatService.CreateAsync(dto);

            return Ok(createdSeat);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{eventSeatId:int}")]
    public async Task<IActionResult> Update(
        int eventSeatId,
        [FromBody] EventSeatDto dto)
    {
        try
        {
            var updated = await _eventSeatService.UpdateAsync(
                eventSeatId,
                dto);

            if (!updated)
                return NotFound("Event seat not found.");

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}