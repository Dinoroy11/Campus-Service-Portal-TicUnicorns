using CampusServicePortal.Modules.Labs.DTOs;
using CampusServicePortal.Modules.Labs.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Labs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LabsController : ControllerBase
{
    private readonly ILabService _labService;

    public LabsController(ILabService labService)
    {
        _labService = labService;
    }

    // GET: api/Labs
    [HttpGet]
    public async Task<ActionResult<List<LabDto>>> GetAllLabs()
    {
        var labs = await _labService.GetAllLabsAsync();

        return Ok(labs);
    }

    // GET: api/Labs/1
    [HttpGet("{labId:int}")]
    public async Task<ActionResult<LabDto>> GetLab(
        int labId)
    {
        var lab = await _labService.GetLabByIdAsync(labId);

        if (lab == null)
            return NotFound(new
            {
                message = "Lab not found."
            });

        return Ok(lab);
    }

    // POST: api/Labs
    [HttpPost]
    public async Task<ActionResult<LabDto>> CreateLab(
        [FromBody] CreateLabDto dto)
    {
        try
        {
            var lab = await _labService.CreateLabAsync(dto);

            return CreatedAtAction(
                nameof(GetLab),
                new { labId = lab.LabId },
                lab);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // PUT: api/Labs/1
    [HttpPut("{labId:int}")]
    public async Task<IActionResult> UpdateLab(
        int labId,
        [FromBody] UpdateLabDto dto)
    {
        try
        {
            await _labService.UpdateLabAsync(
                labId,
                dto);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // GET: api/Labs/1/seats
    [HttpGet("{labId:int}/seats")]
    public async Task<ActionResult<List<LabSeatDto>>>
        GetSeats(int labId)
    {
        try
        {
            var seats =
                await _labService.GetSeatsByLabIdAsync(
                    labId);

            return Ok(seats);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    // POST: api/Labs/seats
    [HttpPost("seats")]
    public async Task<ActionResult<LabSeatDto>>
        CreateSeat(
            [FromBody] CreateLabSeatDto dto)
    {
        try
        {
            var seat =
                await _labService.CreateSeatAsync(dto);

            return Ok(seat);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // GET: api/Labs/1/timeslots
    [HttpGet("{labId:int}/timeslots")]
    public async Task<ActionResult<List<LabTimeSlotDto>>>
        GetTimeSlots(int labId)
    {
        try
        {
            var slots =
                await _labService
                    .GetTimeSlotsByLabIdAsync(labId);

            return Ok(slots);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    // POST: api/Labs/timeslots
    [HttpPost("timeslots")]
    public async Task<ActionResult<LabTimeSlotDto>>
        CreateTimeSlot(
            [FromBody] CreateLabTimeSlotDto dto)
    {
        try
        {
            var slot =
                await _labService
                    .CreateTimeSlotAsync(dto);

            return Ok(slot);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // GET:
    // api/Labs/1/availability?timeSlotId=2&bookingDate=2026-09-10
    [HttpGet("{labId:int}/availability")]
    public async Task<ActionResult<LabAvailabilityDto>>
        GetAvailability(
            int labId,
            [FromQuery] int timeSlotId,
            [FromQuery] DateTime bookingDate)
    {
        try
        {
            var availability =
                await _labService.GetAvailabilityAsync(
                    labId,
                    timeSlotId,
                    bookingDate);

            return Ok(availability);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    // POST: api/Labs/bookings
    [HttpPost("bookings")]
    public async Task<ActionResult<LabBookingDto>>
        CreateBooking(
            [FromBody] CreateLabBookingDto dto)
    {
        try
        {
            var booking =
                await _labService.CreateBookingAsync(dto);

            return Ok(booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // GET: api/Labs/bookings/1
    [HttpGet("bookings/{labBookingId:int}")]
    public async Task<ActionResult<LabBookingDto>>
        GetBooking(int labBookingId)
    {
        var booking =
            await _labService.GetBookingByIdAsync(
                labBookingId);

        if (booking == null)
            return NotFound(new
            {
                message = "Booking not found."
            });

        return Ok(booking);
    }

    // PATCH: api/Labs/bookings/1/cancel
    [HttpPatch("bookings/{labBookingId:int}/cancel")]
    public async Task<IActionResult> CancelBooking(
        int labBookingId)
    {
        try
        {
            await _labService.CancelBookingAsync(
                labBookingId);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }
}