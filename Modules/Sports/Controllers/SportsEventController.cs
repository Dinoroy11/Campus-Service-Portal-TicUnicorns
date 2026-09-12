using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SportsEventController : ControllerBase
{
    private readonly ISportsEventService _sportsEventService;

    public SportsEventController(ISportsEventService sportsEventService)
    {
        _sportsEventService = sportsEventService;
    }

    // Admin sees active + inactive + past events.
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _sportsEventService.GetAllAsync());
    }

    // Students see only active events that have not ended.
    [HttpGet("available")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> GetAvailable()
    {
        return Ok(await _sportsEventService.GetAvailableAsync());
    }

    [HttpGet("{sportsEventId:int}")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> GetById(int sportsEventId)
    {
        try
        {
            var sportsEvent = await _sportsEventService.GetByIdAsync(sportsEventId);
            return sportsEvent == null
                ? NotFound(new { message = "Sports event not found." })
                : Ok(sportsEvent);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateSportsEventDto dto)
    {
        try
        {
            var sportsEvent = await _sportsEventService.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { sportsEventId = sportsEvent.SportsEventId },
                sportsEvent);
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

    [HttpPut("{sportsEventId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int sportsEventId,
        [FromBody] UpdateSportsEventDto dto)
    {
        try
        {
            var sportsEvent =
                await _sportsEventService.UpdateAsync(sportsEventId, dto);

            return sportsEvent == null
                ? NotFound(new { message = "Sports event not found." })
                : Ok(sportsEvent);
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
}
