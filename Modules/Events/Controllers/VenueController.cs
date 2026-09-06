using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Events.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var venues = await _venueService.GetAllAsync();

        return Ok(venues);
    }

    [HttpGet("{venueId:int}")]
    public async Task<ActionResult<VenueDto>> GetById(int venueId)
    {
        var venue = await _venueService.GetByIdAsync(venueId);

        if (venue == null)
            return NotFound("Venue not found.");

        return Ok(venue);
    }

    [HttpPost]
    public async Task<ActionResult<VenueDto>> Create(
        [FromBody] CreateVenueDto dto)
    {
        try
        {
            var createdVenue = await _venueService.CreateAsync(dto);

            return Ok(createdVenue);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{venueId:int}")]
    public async Task<IActionResult> Update(
        int venueId,
        [FromBody] UpdateVenueDto dto)
    {
        try
        {
            var updated = await _venueService.UpdateAsync(
                venueId,
                dto);

            if (!updated)
                return NotFound("Venue not found.");

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}