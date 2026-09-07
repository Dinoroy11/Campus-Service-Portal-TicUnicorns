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

    public SportsEventController(
        ISportsEventService sportsEventService)
    {
        _sportsEventService = sportsEventService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SportsEventDto>>> GetAll()
    {
        var events = await _sportsEventService.GetAllAsync();

        return Ok(events);
    }

    [HttpGet("{sportsEventId:int}")]
    public async Task<ActionResult<SportsEventDto>> GetById(
        int sportsEventId)
    {
        var sportsEvent =
            await _sportsEventService.GetByIdAsync(sportsEventId);

        if (sportsEvent == null)
            return NotFound();

        return Ok(sportsEvent);
    }

    [HttpPost]
    public async Task<ActionResult<SportsEventDto>> Create(
        CreateSportsEventDto dto)
    {
        var sportsEvent =
            await _sportsEventService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { sportsEventId = sportsEvent.SportsEventId },
            sportsEvent);
    }

    [HttpPut("{sportsEventId:int}")]
    public async Task<ActionResult<SportsEventDto>> Update(
        int sportsEventId,
        UpdateSportsEventDto dto)
    {
        var sportsEvent =
            await _sportsEventService.UpdateAsync(
                sportsEventId,
                dto);

        if (sportsEvent == null)
            return NotFound();

        return Ok(sportsEvent);
    }
}