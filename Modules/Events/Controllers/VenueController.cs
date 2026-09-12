using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Events.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VenueController : ControllerBase
{
    private readonly IVenueService _venueService;

    public VenueController(IVenueService venueService)
    {
        _venueService = venueService;
    }

    [HttpGet]
    public async Task<ActionResult<List<VenueDto>>> GetAll()
    {
        return Ok(await _venueService.GetAllAsync());
    }

    [HttpGet("{venueId:int}")]
    public async Task<ActionResult<VenueDto>> GetById(int venueId)
    {
        var venue = await _venueService.GetByIdAsync(venueId);
        return venue == null ? NotFound("Venue not found.") : Ok(venue);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<VenueDto>> Create([FromBody] CreateVenueDto dto)
    {
        try
        {
            return Ok(await _venueService.CreateAsync(dto));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{venueId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int venueId, [FromBody] UpdateVenueDto dto)
    {
        try
        {
            var updated = await _venueService.UpdateAsync(venueId, dto);
            return updated ? NoContent() : NotFound("Venue not found.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
